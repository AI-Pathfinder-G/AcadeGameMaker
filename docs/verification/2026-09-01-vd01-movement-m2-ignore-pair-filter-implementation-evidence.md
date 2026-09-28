# VD-01 M2 Ignored-Pair Cast-Hit Filter Implementation Evidence

- Date: 2026-09-01
- Approved addendum: `docs/specs/work-contracts/2026-09-01-vd01-movement-m2-ignore-pair-hit-filter-addendum.md`
- Requirements: `REQ-MOV-001`, `REQ-MOV-004`, `REQ-MOV-006`; affected `REQ-COM-001`, `REQ-COM-004`, `REQ-COM-005`
- Acceptance criteria traced, partially evidenced: `AC-MOV-001`, `AC-MOV-004`, `AC-MOV-005`; affected `AC-COM-001`, `AC-COM-003`
- Implementer: Terra
- Independent verifier: Luna
- Status: **PASS — implementation verified, P0=0, P1=0, P2=0**

## Integrated behavior

`PlayerMovementController.FindBestHitAt` retains the existing attached-collider Cast and query transaction. After the existing null/self/trigger rejection and before predicate, Q4096 conversion or stable ordering, it skips a raw hit exactly when `Physics2D.GetIgnoreCollision(_collider, hit.collider)` is true. No other runtime behavior changed.

The focused evidence proves that Unity's raw Cast continues to return the ignored collider while the real Movement path removes it, preserving the nonignored regular enemy and environment. Separate tests exercise downward grounding, horizontal wall/dash/X resolution, upward Y resolution, pair-false restoration and fresh-false state. The 30/60/144 replay changes both pair flags at exact logical tick 6 and compares complete tick traces. Ignored/control M3C2 and M3D2 immutable views remain equal.

## Executable evidence

| Run | Result | XML SHA-256 | Traced criteria |
|---|---:|---|---|
| Movement focused | 15/15 passed, 0 failed, 0 skipped | `DD3EFD82FD142F451761A94DB2344192E244F2AB6C4F2D9CD16EF550AF77E921` | `AC-MOV-001`, `AC-MOV-004`, `AC-MOV-005` |
| M3E1A collision retry | 6/6 passed, 0 failed, 0 skipped | `533A3BF37DA1CACADB2B257C15427194F865B046BD9E36791005B0E7140D8206` | affected `AC-COM-001`, `AC-COM-003` |
| CombatUnity focused regressions | 240/240 passed, 0 failed, 0 skipped | `5BAC906FA5863377D3A06325EA36FE5788460C24D9BB519B3596562492863C8A` | affected `AC-COM-001`, `AC-COM-003` |
| Full EditMode | 185/185 passed, 0 failed, 0 skipped | `80881960AD7B61E593A85268D99FD7D7F60E639D80BAA5DEA06395776D1C5E83` | regression |
| Full PlayMode | 274/274 passed, 0 failed, 0 skipped | `C7F75F509D8BD0F054E8AADC130BEF524EA89390C7DAD44BF41AC72DA7DC76F4` | regression |

The first Movement implementation run found two fixture-only assumptions: an unisolated ground setup and an over-tight two-Q4096 ceiling threshold. After isolation and semantic assertions, the second run reached 14/15; its last failure showed pair-false restoration succeeded but the fixture had already fallen into overlap. Widening the initial probe gap without changing runtime produced the final 15/15 result.

## Boundary checks

- production adds zero Cast, overlap, raycast or `SyncTransforms` calls and no `IgnoreCollision` mutation;
- one new read-only `GetIgnoreCollision` branch exists in the shared raw-hit loop;
- no public surface, serialization, assembly, package, layer, project setting, time, RNG or discovery change;
- raw direct-cast fixtures assert count below the retained 32-entry buffer;
- protected `Assets/Scenes/MovementSandbox.unity`, `ProjectSettings/URPProjectSettings.asset` and `ProjectSettings/SceneTemplateSettings.json` remained outside the implementation allowlist and were not staged or restored.

## Ollama utilization record

Kimi K3 supplied the bounded minimal-filter and fixture decomposition, accepted only after Terra/GPT screening. GLM 5.2 supplied adversarial ground/wall/dash/axis/lifecycle/cadence cases; unsourced mechanics were rejected. MiniMax M3 produced no final answer in two bounded calls and was replaced by Terra fixtures with Luna review. No Ollama model received repository material or decision authority.

## Integration decision

Luna independently inspected the complete allowlisted diff, shared query-path reachability, pair-state tick boundary, raw-cast oracle, cadence transition, M3C2/M3D2 equality, source scans and all five XML artifacts. The final result is PASS with `P0=0`, `P1=0`, `P2=0`; Sol accepts the implementation. This reopens only M3E1A's passed collision gate and the collision-neutral M3E1B0 stage. It does not authorize M3E1B1 or close a full acceptance criterion.
