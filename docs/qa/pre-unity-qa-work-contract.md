# Work Contract: Pre-Unity QA artifacts

- Owning spec and revision: `VD-11`, Approved 2026-08-24
- Assigned by: Astra
- Implementer: GPT Terra (`gpt-5.6-terra`)
- Independent verifier: GPT Luna (`gpt-5.6-luna`)
- Requirement IDs: `REQ-PLAT-012`, `REQ-PLAT-013`, `REQ-PLAT-014`
- Acceptance-criterion IDs: `AC-PLAT-010`, `AC-PLAT-011`, `AC-PLAT-012`
- Allowed files/directories: `qa/README.md`, `qa/schema/`, `qa/catalog/`, `qa/coverage/`, `qa/tools/`
- Forbidden files/directories: every path outside `qa/`; especially `Assets/`, `Packages/`, `ProjectSettings/`, `docs/canon/`, `docs/adr/`, existing specs, `.git/`, `third_party/`
- Public interface or data contract: VD-11 schemaVersion 1 catalog and validator exit/diagnostic contract
- Cross-part invariants: existing REQ/AC values are referenced, never rewritten; no gameplay value invention; unresolved exact behavior remains blocked by approved OD; no network, secret, Unity or external module
- Rollback point: main commit `c29b961`
- Required implementation evidence: validator normal run, `-SelfTest`, JSON parse, 68 unique required AC, coverage report, allowed-path diff
- Required independent verification evidence: GPT Luna report citing AC-PLAT-010~012 and independently reproduced defects; historical Ollama defects are reference-only and are not a current input
- Integration order and dependencies: schema→manifest→catalog→validator→self-test→Luna review→Astra integration
- Historical approval: Sol, Approved 2026-08-24; future approval and integration follow Astra under ADR-0032
