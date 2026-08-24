# Work Contract: Pre-Unity QA artifacts

- Owning spec and revision: `VD-11`, Approved 2026-08-24
- Assigned by: Sol
- Implementer: MiniMax M3 isolated proposal (partial); GPT contract correction and completion required
- Independent verifier: Luna
- Requirement IDs: `REQ-PLAT-012`, `REQ-PLAT-013`, `REQ-PLAT-014`
- Acceptance-criterion IDs: `AC-PLAT-010`, `AC-PLAT-011`, `AC-PLAT-012`
- Allowed files/directories: `qa/README.md`, `qa/schema/`, `qa/catalog/`, `qa/coverage/`, `qa/tools/`
- Forbidden files/directories: every path outside `qa/`; especially `Assets/`, `Packages/`, `ProjectSettings/`, `docs/canon/`, `docs/adr/`, existing specs, `.git/`, `third_party/`
- Public interface or data contract: VD-11 schemaVersion 1 catalog and validator exit/diagnostic contract
- Cross-part invariants: existing REQ/AC values are referenced, never rewritten; no gameplay value invention; unresolved exact behavior remains blocked by approved OD; no network, secret, Unity or external module
- Rollback point: main commit `c29b961`
- Required implementation evidence: validator normal run, `-SelfTest`, JSON parse, 68 unique required AC, coverage report, allowed-path diff
- Required independent verification evidence: Luna report citing AC-PLAT-010~012 and screened Ollama defects
- Integration order and dependencies: schema→manifest→catalog→validator→self-test→Luna review→Sol integration
- Sol approval/date: Approved, 2026-08-24
