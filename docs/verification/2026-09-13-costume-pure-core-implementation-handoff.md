# 2026-09-13 costume pure-core implementation handoff

- Status: implementation handoff; not independent acceptance.
- Contract: [costume presentation and sprite production](../specs/work-contracts/2026-09-13-costume-presentation-and-sprite-production.md).
- Scope: only the contract's first-slice `Runtime/Costumes`, non-Unity `Runtime/Presentation`, matching EditMode assemblies, and this documentation index/evidence hunk.

## Delivered mapping

`REQ-COST-001/002/011` is represented by `CostumeCatalogV1` and the immutable 36-row Seryeong inventory. It follows `images/sprites/costumes-v2/costume-inventory.json`: the source's historical `story` ID is retained as `seryeong.costume.story.v1`; all rows are `ProductionPending`, include a source SHA-256 and fixed reference-hair metadata, and therefore expose `NoAcceptedDefault` rather than a fabricated default.

`REQ-COST-003/004` has revision-bound immutable state, locked/pending rejection, a single-use opaque candidate token bound to the exact catalog instance, and deterministic canonical value codec. The internal closed production issuer allowlist provides authored-default, progression, and entitlement lanes with opaque capabilities, exact replay/conflict handling, and monotonic per-lane revisions. There is no unlock-all path.

`REQ-COST-005/006` provides byte-in/byte-out canonical encoding, integrity verification, defensive recovery inputs, and pure primary/previous/temp selection. Recovery bootstraps only accepted defaults, preserves valid unlocks, falls back invalid current selections, and removes an invalid current mapping when no accepted default exists. File IO and quarantine remain separate child work.

`REQ-COST-007/008/009/012` has an atomic portrait/atlas/clip-map binding value, completed-snapshot swap gate, action-phase-preserving snapshot seam, one-pixel-bounded motion descriptor, and settings/character view models. There is no Unity renderer, Animator, scene, or simulation call.

## Focused evidence

The focused sources and isolated .NET harness cover inventory/pending state, issuer lanes and replay conflicts, monotonic revisions, same-revision foreign catalogs, tamper rejection, no-default recovery omission, defensive recovery bytes, and default binding fail-closed behavior.

Static inspection found no `System.IO`, Unity references, Profile/Run references, media writes, or production unlock-all route in the new runtime folders. Unity was intentionally not launched because concurrent work is active and this handoff needs independent Luna execution before `AC-COST-011/012` can be claimed.

## Deferred / limits

This is not an accepted visual inventory: no portraits, gameplay atlas, clip map, native-scale review, media processing, filesystem recovery, Unity binding, or full regression result is claimed. `AC-COST-006`, `AC-COST-008`, `AC-COST-010`, `AC-COST-011`, and `AC-COST-012` remain for their dedicated child work and independent review.
