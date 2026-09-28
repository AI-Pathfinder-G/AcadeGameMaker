# Costume CUA static-review closure amendment — Luna re-gate

- Date: 2026-09-27
- Reviewer: Luna (`gpt-5.6-luna`)
- Review type: independent normative amendment re-gate; no implementation and no Unity execution
- Approved contract SHA-256: `600E1905D285354256365C021234C3470BAA6D86BC770779E0794B3D6D1548B1`
- Sol closure proposal SHA-256: `C3315DE9CD5D6F3D1077B0258FD7C1DADD7BC9CDF5D905EE84EE093317C9F3F1`
- Prior Luna implementation review SHA-256: `743ECF1AD040044954D89A4FC9746A3A144F68CE4BA7DAC48B958A83AF6B0639`

## Verdict

**PASS — P0=0, P1=0, P2=0 for the amendment.** All four prior P1 findings and both P2 findings are normatively and implementably closed. Terra is authorized to make only the exact four-file correction delta described below. This PASS does **not** authorize Unity execution or implementation acceptance: after Terra changes the files, Luna must re-review the replacement hashes and report a separate pre-execution gate; Astra must then accept that gate before any run.

## Closure findings

### Snapshot provenance — closed

The amendment removes every `TrySelect` overload accepting a snapshot. Selection receives only `CostumeUnityMediaPackageV1`; the sole writer of the private accepted-observation slot is the explicit instance-bound `ICostumeUnityCompletedSnapshotPortV1`. First publication requires a port-accepted observation, and later observations are monotonic with exact same-tick identity, conflict rejection, and projection-failure closure. “Future/uncommitted” is correctly defined at this isolated boundary as a value with no route into selection; live-owner call timing remains a later integration contract. This closes the prior public snapshot bypass without adding a clock, owner lookup, token override, or pure-core widening.

### Pure swap and immutable publication — closed

The amended sequence requires `TryCreateBinding`, independent package validation, exact `(ActionId,Facing)` frame resolution, current tuple/state correlation, and pure `CostumePresentationServiceV1.TrySwap` before candidate construction and synchronous CIO save. Replacement uses current/proposed bindings and requires exact structural equality of the committed binding. First synthetic publication is explicitly limited to an empty authoritative current state, an accepted port observation, and a complete proposed package, then uses the approved reflexive `TrySwap(proposed, proposed, snapshot, out committed)` bootstrap. The returned committed binding—not an unvalidated proposed object—enters the immutable tuple. All fallible work precedes one reference assignment; projection follows it. This closes the missing pure gate and preserves the durable/new-tuple versus `ReloadRequired` semantics.

### Package validation matrix — closed

The amendment requires independent validation of package and binding identity/completeness, exact hashes/manifests, action/facing completeness, payload and reference lifetime, filter/PPU, pivot/baseline, geometry, canonical bytes, and preservation of the old tuple for each isolated mutation. Constructor exceptions are explicitly not accepted as a substitute for independent `Validate` cases. This is sufficient to close the prior validator-assumption gap and maps directly to `AC-CUA-002/003/008` without changing REQ/AC meaning or canonical property order.

### Save/projection/snapshot/mechanics evidence — closed

The amendment mandates two-package first/replacement coverage, concrete CIO-only save correlation, wrong-primary and projection faults, terminal mutation blocking, durable/new authority after post-save projection failure, the complete action/facing/frame and tick matrix, and an isolated mechanics/physics-owner probe that the adapter never receives. It requires individually named tests and zero failed/skipped/inconclusive counts before execution. The required cases cover the missing evidence boundaries rather than weakening them.

### Widened bounds — closed

The normative rule requires `checked((long)x + (long)width)` and corresponding top arithmetic, plus maximum-integer and exact-edge tests. This removes the unchecked `int` overflow bypass while preserving the existing geometry contract.

### Duplicate rectangles — closed without product change

For the synthetic-only schema-v1 slice, repeated `(x,y,width,height)` rectangles within one `(actionId,facing)` clip are rejected; cross-clip reuse remains allowed. Intentional holds are explicitly deferred to a separately approved schema-v2 real-media slice. This is conservative, deterministic, does not change current player behavior or real media semantics, and leaves canonical JSON property order/schema version/hash inputs unchanged. The contract now states the rule normatively, so no unresolved product choice remains.

## Allowlist and dependency safety

The correction delta is limited to:

1. `CostumeUnityMediaPackageV1.cs` — independent binding checks, widened bounds, within-clip duplicate rejection;
2. `CostumeUnityPresentationAdapterV1.cs` — private observation slot, snapshot-free selection, port-only ingestion, pure swap/bootstrap, tuple comparison, terminal guard;
3. `CostumeUnityPresentationAdapterV1Tests.cs` — replacement fixture and complete required matrices;
4. `CostumeUnityViewPresenterV1Tests.cs` — public-surface/port/no-global/no-mechanics/terminal probes;
5. the already allowlisted implementation and independent-review evidence records after results exist.

The presenter runtime source, both asmdefs/metas, pure costume/presentation core, CIO, catalog, scenes, prefabs, media, packages, ProjectSettings, and existing owners remain byte-identical. Any request outside these paths is a hard stop requiring Astra to revise the allowlist. The amendment preserves `REQ-CUA-001..010`, `AC-CUA-001..009`, synthetic-only `0/432` real-media status, and no user product decision.

## Authorization and stop conditions

**Correction authorization: YES, exact delta only.** Terra may implement the amendment, but must not run Unity or claim `Verified`. Stop if any correction needs an unnamed owner or non-allowlisted path, a snapshot-selection overload/token/clock/global lookup, a detached save result/delegate, a pure `TrySwap` bypass, partial publication, accepted overflow/duplicate, canonical-byte/schema change, mechanics drift, real media/catalog promotion, or nonzero required test counts. After correction, Luna must verify exact replacement source/test/meta/asmdef hashes and P0/P1/P2=0; only then may Astra authorize the focused PlayMode run followed by fresh unfiltered full EditMode and PlayMode runs.

