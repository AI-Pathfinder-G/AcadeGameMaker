# VD-09 M5D7P-B Unity UI package baseline — Luna pre-gate

- Review date: 2026-09-20
- Reviewer: Luna (`gpt-5.6-luna`), independent contract review
- Contract: `docs/specs/work-contracts/2026-09-20-vd09-m5d7p-b-ui-package-baseline.md`
- Reviewed against: `AGENTS.md`, ADR-0032, `docs/agent-operating-model.md`, the M5D7O Verified contract/evidence/post-review, current package manifest/lock and ProjectVersion, installed Unity `6000.6.0f1` BuiltInPackages metadata, and the latest available Unity Editor logs.
- Scope: adverse pre-implementation review only. No package, runtime, test, contract, or ProjectSettings changes were made.

## Initial verdict (superseded by the amendment re-review below)

**CONDITIONAL — P0=0, P1=1, P2=2. Do not recommend approval yet.**

The direct built-in package choice is correct: Unity `6000.6.0f1` locally contains uGUI `2.6.0`; that package contains the TMP runtime assembly/source; the separate TextMeshPro package is a deprecated shim that forwards to uGUI. The lock expectations are consistent with the installed package metadata. The contract's current AC-M5D7PB-003 nevertheless requires a consumer-level `TMPro` namespace compile while its allowlist prohibits adding or changing any source/assembly definition. Existing project sources do not consume TMP, so ordinary project compilation cannot prove that assertion. Resolve this evidence/allowlist mismatch before the contract is Approved.

## Direct evidence

- `ProjectSettings/ProjectVersion.txt` pins `6000.6.0f1 (f7f8ed4d1e24)`.
- Current `Packages/manifest.json` does not declare `com.unity.ugui` or `com.unity.textmeshpro`.
- The installed `BuiltInPackages/com.unity.ugui/package.json` reports version `2.6.0` and declares exactly these module dependencies: `com.unity.modules.ui`, `com.unity.modules.imgui`, `com.unity.modules.audio`, `com.unity.modules.physics2d`, and `com.unity.modules.physics`, each `1.0.0`.
- The installed `BuiltInPackages/com.unity.textmeshpro/package.json` reports version `5.0.0`, `type: shim`, says it is no longer supported and that TextMeshPro functionality is included in uGUI, and declares only `com.unity.ugui: 2.0.0`.
- uGUI's installed `Runtime/TMP/Unity.TextMeshPro.asmdef` names the assembly `Unity.TextMeshPro` and is auto-referenced. Its `Runtime/TMP` source contains `namespace TMPro`. Thus the TMP implementation is physically included in the exact local uGUI package, not merely advertised by keywords.
- Current `Packages/packages-lock.json` has no uGUI/TMP entry. It already has `com.unity.modules.physics2d` as a direct built-in at depth 0 and `com.unity.modules.physics`/`ui`/`imgui` as transitive built-ins. The M5D7P-B lock acceptance must allow only dependency-depth/addition changes consistent with the new uGUI edge; it must not re-root or version-substitute existing packages.
- No project-owned C# source currently imports/uses `TMPro`; M5D7O explicitly treats `TMPro` as forbidden in its controller. Existing source therefore provides no consumer compile witness.
- Latest available `Logs/Editor.log` is timestamped 2026-09-14, before this package-baseline contract; its package section says it restored the prior cached 23-package state and contains no uGUI/TMP package-resolution evidence. The global Editor logs are older still. They cannot establish a successful post-change resolution. A fresh package-resolution smoke after implementation remains necessary.

## Acceptance-criterion review

| Criterion | Result | Finding |
|---|---|---|
| AC-M5D7PB-001 | **PASS as proposed** | One direct manifest entry `com.unity.ugui: 2.6.0`, with no `com.unity.textmeshpro`, matches the installed built-in package. |
| AC-M5D7PB-002 | **PASS as proposed** | Expected uGUI lock entry is `version=2.6.0`, `source=builtin`, `depth=0`, with the five exact module dependencies from the installed `package.json`; no shim lock entry. Because `physics2d` is already direct, it should remain depth 0. Existing `ui` and `physics` may have shallower depth after resolution; `audio` is newly introduced. Do not treat these mechanically expected module graph updates as unrelated dependency changes. |
| AC-M5D7PB-003 | **BLOCKED — P1** | Installed metadata and source establish that uGUI carries `Unity.TextMeshPro`/`TMPro`, but no project consumer currently imports it. The proposed allowlist prohibits C# and asmdef additions, so the criterion's consumer-level “TMP namespaces compile through that dependency” cannot be demonstrated by a normal project compile. Metadata inspection or successful compilation of Unity's own package is weaker than proving the project's downstream assembly can bind the namespace. The criterion must either (a) be narrowed here to local built-in path/version, package registration, absence of resolution/version substitution, and successful package-assembly compile while deferring consumer namespace compilation to M5D7P authored-code acceptance; or (b) explicitly define a disposable, non-repository/in-memory consumer compile probe and its exact references/output evidence. Do not expand the committed implementation allowlist with a permanent probe file for this package-only unit. |
| AC-M5D7PB-004 | **PASS as a future gate, with evidence caveat** | Full EditMode and PlayMode plus resolution smoke are proportionate and preserve the M5D7O Verified baseline. Require fresh XMLs, each with failure/skip/inconclusive zero, and Luna P0=0/P1=0. The September 14 log is historical only; no current smoke pass is present yet. Diff must remain limited to manifest, lock, and authorized evidence/index/contract status documentation. |

## Package-resolution and offline risk

The selected version/source is locally available under the pinned Editor's `PackageManager/BuiltInPackages` tree. All five declared dependencies are Unity built-in modules; no registry URL, Git dependency, file path, override, or deprecated shim is needed. Therefore this addition should not require a network fetch for uGUI or its declared module dependencies. This is a strong static basis, not a substitute for running Unity's package resolver once after the manifest change. Stop if the resolver selects any source/version other than the pinned Editor's built-in `2.6.0`, attempts to substitute a registry package, reports an error, or rewrites an unrelated direct dependency. The Unity license/editor startup is a separate operational prerequisite from package availability.

## Severity and required resolution

- **P0: 0**
- **P1: 1 — AC003 cannot be evidenced under the current allowlist.** Astra must amend the contract's acceptance evidence or authorize a temporary, non-repository compile probe before marking it Approved. This review does not amend the contract.
- **P2: 2**
  1. The current Unity logs predate the package addition and cannot serve as post-resolution evidence; capture a fresh smoke log and lock after implementation.
  2. State the expected transitive lock delta explicitly enough to distinguish required depth/addition changes (`audio`, `physics`, `ui`, and existing `physics2d`) from prohibited unrelated rewrites. Current metadata does support the expected exact dependency set.

**Gate recommendation:** hold at `Review` until the AC003 compile-evidence mismatch is resolved. No end-user/product decision is required; this is a contract/testability correction.

## Amendment re-review — Astra contract revision

- Re-review date: 2026-09-20
- Scope: only the amended M5D7P-B contract clauses and their closure of findings above. The package files remain unchanged; this reviewer did not modify the contract.

### Finding disposition

- **Former P1 / AC-M5D7PB-003 — CLOSED.** The revised criterion now requires Unity to register and compile the auto-referenced `Unity.TextMeshPro` assembly included by installed uGUI 2.6.0, with `TMPro` present, and requires package resolution to report no error or version substitution. It explicitly defers a project-owned consumer import to the authored-presentation contract. This is consistent with the package-only allowlist and the installed metadata/source evidence: `Unity.TextMeshPro.asmdef` is auto-referenced and the included runtime source declares `namespace TMPro`. AC003 can be evidenced without adding committed C# or an asmdef to M5D7P-B. The later authored-presentation unit must still prove the actual project consumer compile.
- **Former P2 / lock-delta ambiguity — CLOSED.** REQ-M5D7PB-002 now names the expected graph changes: uGUI depth 0; audio depth 1; ui and physics may become depth 1; imgui stays depth 1; direct physics2d stays depth 0. This matches the installed uGUI package metadata and the current lock's existing direct/transitive roots. AC002 still requires uGUI's exact five declared module dependencies and excludes the shim.
- **Former P2 / stale Unity logs — CLOSED as a pre-gate concern; retained as a post-implementation evidence condition.** The available 2026-09-14 Unity log predates this package change and is not claimed as a resolution pass. AC003/AC004 require a fresh package-resolution smoke and post-change Unity registration/compile evidence before implementation acceptance. This is the correct phase boundary: absence of a not-yet-run implementation smoke is not a contract-readiness defect.

### Re-review verdict

| Severity | Open findings |
|---|---:|
| P0 | 0 |
| P1 | 0 |
| P2 | 0 |

**PASS — recommend Astra approve M5D7P-B.** The approved-only implementation gate remains intact: this recommendation accepts the bounded package contract, not an implementation or package-resolution result. Terra must make only the allowlisted manifest/lock/documentation changes after approval; fresh resolution, full EditMode, and full PlayMode evidence must still pass with failure/skip/inconclusive zero, and Luna must independently report P0=0/P1=0 before Astra marks the unit Verified.

## Dirty working-tree baseline amendment re-review

- Re-review date: 2026-09-20
- Scope: review of Astra's second contract amendment freezing the existing package-file working-tree state and measuring AC-M5D7PB-004 against that state rather than Git HEAD. No contract or package file was changed by Luna.

### Finding disposition

- **User-change preservation and scope baseline — CLOSED.** The contract now explicitly says the task is measured against the frozen pre-implementation working-tree state, preserves pre-existing user/Unity lockfile changes, and does not attribute those changes to M5D7P-B. I independently recomputed the current files: `Packages/manifest.json` is SHA-256 `7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25`; `Packages/packages-lock.json` is SHA-256 `C0DA832620B8DB22C1E607D3E2638680C29186A871F55FA6FA5B453DD27432A9`. Both match the contract's frozen values. The current manifest is clean relative to Git, while the lock has a broad existing HEAD diff; that lock diff is now correctly treated as baseline, not as this unit's implementation.
- **Exact-delta acceptance and stop condition — CLOSED.** REQ-M5D7PB-004 and AC-M5D7PB-004 now require a semantic before/after comparison that permits only the direct uGUI manifest addition and resolver-required uGUI dependency-graph lock delta, plus authorized evidence/docs. The earlier expected lock graph remains explicit. Existing broad lock differences therefore neither mask unrelated new mutations nor trigger a false stop solely because HEAD differs. Any post-baseline direct package change or lock delta outside the uGUI dependency graph remains a stop condition under the unchanged resolver/rollback rules.
- **Rollback guard — implementation procedure.** Because the lock's exact baseline is user/Unity working-tree state rather than Git HEAD, Terra must capture byte-for-byte snapshots of both package files before invoking Unity or editing either file, verify them against the two frozen hashes, and on a stop restore from those snapshots—not from Git HEAD. This is the operational meaning of restoring only this unit's resulting lockfile delta and does not widen the committed allowlist.
- **Tested precondition — CLOSED.** The evidence record ties the frozen dirty lock state to the final M5D7O full EditMode and PlayMode passes. M5D7P-B must still rerun the required package smoke and both full suites after its own package delta; the prior results establish baseline, not post-change acceptance.

### Re-review verdict

| Severity | Open findings |
|---|---:|
| P0 | 0 |
| P1 | 0 |
| P2 | 0 |

**PASS — maintain M5D7P-B's Approved status.** The second amendment sufficiently preserves existing working-tree changes and defines a measurable stop boundary. This is approval of the bounded package contract only; it is not verification of Terra's implementation. The byte-snapshot/hash check is a mandatory safe preflight before any resolver write, followed by the contract's exact semantic-delta comparison and fresh AC-M5D7PB-003/004 evidence.
