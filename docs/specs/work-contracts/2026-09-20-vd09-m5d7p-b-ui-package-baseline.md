# VD-09 M5D7P-B Unity UI package baseline

- Status: Verified
- Owner and final approval authority: Astra
- Intended implementer after approval: Terra
- Independent reviewer: Luna
- Dependency: Unity Editor `6000.6.0f1`; M5D7O Verified
- Parent requirements: `REQ-UX-009`, `REQ-UX-014`, `REQ-PLAT-002`
- Proposed acceptance IDs: `AC-M5D7PB-001` through `AC-M5D7PB-004`
- Astra approval: Approved on 2026-09-20 after Luna's amendment re-review
  reported PASS (`P0=0`, `P1=0`, `P2=0`). Approval authorizes only the
  allowlisted package/evidence changes; it does not approve authored UI.

## Purpose and evidence baseline

Declare the one direct UI package dependency required before authored uGUI/TMP
hub work. The installed Unity `6000.6.0f1` distribution provides built-in
`com.unity.ugui` version `2.6.0`; its package metadata lists TextMeshPro as an
included capability. The same Editor contains `com.unity.textmeshpro` version
`5.0.0` only as an unsupported shim whose sole dependency is
`com.unity.ugui >= 2.0.0`.

The project currently declares neither package directly. This contract adds
only direct `com.unity.ugui: 2.6.0`. It must not add the deprecated
`com.unity.textmeshpro` shim, a registry override, Git/file package, UI scene,
TMP resources, font asset, component, script, prefab, or new input owner.

## Requirements

- **REQ-M5D7PB-001:** declare exactly one direct built-in dependency,
  `com.unity.ugui` version `2.6.0`, in `Packages/manifest.json`.
- **REQ-M5D7PB-002:** allow Unity `6000.6.0f1` to resolve the corresponding
  lock entry from its installed built-in package and preserve the package's
  exact declared module dependencies. The expected graph delta is: new direct
  uGUI at depth 0; new `com.unity.modules.audio` at depth 1; existing
  `com.unity.modules.ui` and `com.unity.modules.physics` may become depth 1;
  existing `com.unity.modules.imgui` remains depth 1; already-direct
  `com.unity.modules.physics2d` remains depth 0.
- **REQ-M5D7PB-003:** do not declare `com.unity.textmeshpro`; use the TMP
  functionality included by uGUI 2.6.0 in the later authored-presentation
  unit.
- **REQ-M5D7PB-004:** make no runtime, test, scene, prefab, asset, ProjectSettings,
  generated input, or unrelated package change and preserve the verified test
  baseline. Task scope is measured against the frozen pre-implementation
  working-tree package baseline below, not against Git HEAD; pre-existing
  user/Unity lockfile changes remain untouched and are not attributed to this
  unit.

## Acceptance criteria

- **AC-M5D7PB-001:** after Unity resolution, `manifest.json` contains exactly
  one `com.unity.ugui` entry with value `2.6.0`, and no
  `com.unity.textmeshpro` entry.
- **AC-M5D7PB-002:** `packages-lock.json` records `com.unity.ugui` at depth `0`,
  source `builtin`, version `2.6.0`, and only the dependencies declared by the
  installed package metadata; no shim lock entry is present.
- **AC-M5D7PB-003:** the resolved package metadata is the installed Editor's
  uGUI 2.6.0 package; Unity registers and compiles its auto-referenced
  `Unity.TextMeshPro` package assembly containing the `TMPro` namespace; and
  the package manager reports no resolution error or version substitution.
  A project-owned consumer import is deliberately deferred to the later
  authored-presentation contract because this package-only unit permits no C#
  or asmdef change.
- **AC-M5D7PB-004:** package-resolution smoke, full EditMode, and full PlayMode
  complete with failure/skip/inconclusive zero; Luna reports `P0=0` and
  `P1=0`; a before/after package comparison proves that this unit changed only
  the direct uGUI manifest entry and its exact resolver-required lock delta,
  plus authorized evidence/docs.

### Traceability

| Requirement | Acceptance evidence |
|---|---|
| `REQ-M5D7PB-001` | `AC-M5D7PB-001`, `AC-M5D7PB-002` |
| `REQ-M5D7PB-002` | `AC-M5D7PB-002`, `AC-M5D7PB-003` |
| `REQ-M5D7PB-003` | `AC-M5D7PB-001`, `AC-M5D7PB-003` |
| `REQ-M5D7PB-004` | `AC-M5D7PB-004` |

## Proposed implementation allowlist

- `Packages/manifest.json`
- `Packages/packages-lock.json`
- this contract, its Luna pre-gate/implementation/post-review evidence, and a
  minimal `docs/README.md` index entry

No C# source, asmdef, test source, scene, prefab, UI/TMP/font asset,
ProjectSettings, generated file, or other package manifest entry is allowed.

## Stop and rollback conditions

Stop if Unity resolves a different uGUI version/source, requires the deprecated
TMP shim, rewrites an unrelated direct dependency, cannot resolve from the
installed Editor, or requires a new user-facing product decision. Rollback
removes only the direct uGUI manifest entry, restores the resulting lockfile
delta, and removes this unit's evidence/index entries.

## Review record

- Local installed-package metadata was read from
  `C:/Program Files/Unity/Hub/Editor/6000.6.0f1/Editor/Data/Resources/PackageManager/BuiltInPackages`.
- uGUI reports version `2.6.0` and lists TextMeshPro in its capabilities.
- the TextMeshPro `5.0.0` package declares `type: shim`, states that it is no
  longer supported, and redirects functionality to uGUI.
- Luna's first pre-gate reported `P0=0`, `P1=1`, `P2=2`: the original AC003
  required a project consumer compile while the allowlist prohibited source
  changes. Astra closed that evidence mismatch by limiting this unit to
  Unity's registration/compilation of the included auto-referenced
  `Unity.TextMeshPro` package assembly; actual project consumer compilation is
  reserved for the authored-presentation contract. Astra also made the exact
  expected transitive lock-depth delta explicit.
- Terra's first implementation preflight found that `Packages/packages-lock.json`
  already differed broadly from Git HEAD before this unit, while the manifest
  remained unchanged. Astra did not revert or claim those pre-existing
  changes. The task baseline is frozen at manifest SHA-256
  `7EE76CE89F845F718D861C1015E07C02F4C9A469E5ECCA71E396E5363D97DD25`
  and lock SHA-256
  `C0DA832620B8DB22C1E607D3E2638680C29186A871F55FA6FA5B453DD27432A9`.
  Terra must preserve that complete baseline and submit a direct before/after
  semantic comparison; any resolver delta beyond the approved uGUI dependency
  graph remains a stop condition. The final M5D7O Unity runs already passed on
  this working-tree lock state, so it is a tested precondition rather than an
  unverified change introduced by M5D7P-B.

No user product decision is required. Luna closed the initial AC003 evidence
mismatch and lock-delta ambiguity, and Astra approved this bounded package
unit on 2026-09-20. Terra then implemented only the allowlisted package delta.
Luna independently matched both frozen baseline hashes, reproduced the exact
semantic before/after comparison, and verified package resolution plus full
EditMode `688/688` and PlayMode `805/805` with failure/skip/inconclusive zero.
Its post-review reports `PASS`, `P0=0`, `P1=0`, `P2=0`; Astra therefore accepts
`AC-M5D7PB-001..004` and marks this contract `Verified` on 2026-09-20.
