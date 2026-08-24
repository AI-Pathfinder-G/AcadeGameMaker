# Agent Operating Rules

Read [docs/README.md](./docs/README.md), [CONTEXT.md](./CONTEXT.md), the applicable ADRs in `docs/adr/`, the applicable approved specs in `docs/specs/`, and [docs/agent-operating-model.md](./docs/agent-operating-model.md) before working.

## Documentation authority

- `CONTEXT.md` owns project vocabulary only; it must not contain implementation requirements.
- `docs/canon/` owns the current game, narrative, and art truth.
- `docs/adr/` records consequential decisions and their rationale; superseded ADRs remain as history.
- `docs/specs/` owns normative, testable behavior. Implementation may begin only for a spec whose status is `Approved`.
- `wiki/` is a publication and navigation view. It never overrides canon, ADRs, or specs.
- Every implementation change must cite requirement IDs and every verification result must cite acceptance-criterion IDs.

## Allowed model roster

- GPT-5.6 Sol (`gpt-5.6-sol`)
- GPT-5.6 Terra (`gpt-5.6-terra`)
- GPT-5.6 Luna (`gpt-5.6-luna`)
- Ollama Cloud GLM 5.2 (`glm-5.2:cloud`)
- Ollama Cloud MiniMax M3 (`minimax-m3:cloud`)
- Local Ollama Qwen3.8 (`qwen3.8:latest`)
- Ollama Cloud Kimi K3 (`kimi-k3:cloud`)

Do not assign project work to other models unless the user changes this roster.

## Authority

- Sol owns multi-agent orchestration, role allocation, system-wide design, interface contracts, cross-part decisions, conflict resolution, and final integration.
- Terra owns the design and implementation of individual subsystems and content parts inside contracts established by Sol.
- Luna owns test design, independent review, regression checks, build verification, and well-specified support work.
- Ollama models have no decision, approval, merge, or canon authority. They may produce drafts, classifications, reports, repetitive content, and isolated patch proposals.
- Ollama output must be screened by Terra or Luna. Any critical or canonical adoption requires Sol's approval.

## Critical work that must stay with GPT

- Sol defines and integrates player movement, `무게 전이`, combat, run assembly, save/progression, and public API contracts; Terra designs and implements each unit.
- Sol owns humanity choices, character motivations, chapter canon, heroine arc, endings, and thematic integration; Terra develops assigned chapter or feature units.
- Sol alone approves Unity project settings, cross-cutting schemas, architecture, licensing decisions, destructive changes, and integration.

## Safe Ollama delegation

- Asset cataloging, tagging, naming, metadata tables, and duplicate checks
- Log summarization, file search summaries, test-case drafts, checklist generation, and formatting
- Dialogue variants, room/event variants, backlog decomposition, and non-canonical research drafts
- Full-scene dialogue drafts when explicitly assigned; GLM or GPT must review them and Sol alone may accept canonical text
- Small utilities or data-transform proposals that do not alter core runtime contracts

Cloud models must never receive secrets, credentials, personal data, or other sensitive local material.
