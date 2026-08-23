# Multi-Agent Operating Model

## Principle

Sol orchestrates the whole program and integrates every consequential decision. Terra designs and implements bounded units inside Sol-defined contracts. Luna verifies quality, while Ollama models increase throughput on reversible, well-specified work and submit outputs for GPT review.

## Roles

| Model | Primary work | May decide | Must not decide |
|---|---|---|---|
| GPT-5.6 Sol (`gpt-5.6-sol`) | Multi-agent orchestration, role allocation, system-wide design, contracts, cross-part resolution, final integration | Program architecture, canonical direction, integration and acceptance | — |
| GPT-5.6 Terra (`gpt-5.6-terra`) | Unit-level feature and content design, Unity C# implementation, implementation-owned unit tests and technical notes | Local design and implementation details inside a Sol-approved contract | Reallocating roles, changing cross-part contracts, independent acceptance, final integration |
| GPT-5.6 Luna (`gpt-5.6-luna`) | Independent test design, acceptance review, regressions, build checks and well-specified support | Verification details and non-architectural support choices | Feature implementation ownership, core architecture, canon, or integration decisions alone |
| GLM 5.2 Cloud (`glm-5.2:cloud`) | Long-context summaries, alternatives, backlog decomposition, documentation drafts | Nothing canonical | Architecture, balance, public APIs, canon |
| MiniMax M3 Cloud (`minimax-m3:cloud`) | Asset and screenshot triage, tags, dialogue and content variants | Nothing canonical | Art direction, character motivation, ending meaning |
| Qwen3.8 Local (`qwen3.8:latest`) | File and log summaries, data conversion, test drafts, naming and metadata chores | Nothing canonical | Core code, schema changes, merges |

## Work gates

1. **Orchestration — Sol:** Decompose the objective, assign models, define dependencies, and set the integration order.
2. **Contract — Sol:** Define acceptance criteria, interfaces, allowed files, forbidden areas, and cross-part invariants for each unit.
3. **Unit design and implementation — Terra:** Design and implement the assigned subsystem or content part and its unit tests inside the approved contract. Ollama may supply drafts and Luna may advise on testability without becoming the implementer.
4. **Independent review — Luna:** Luna designs or executes acceptance checks, reviews Unity lifecycle, serialization, null handling, regressions, and evidence against AC IDs. Terra resolves unit-level defects without changing Sol's contracts; Luna does not approve its own implementation work.
5. **Integration — Sol:** Resolve cross-part conflicts and integrate only after tests and a playable scene check pass, with a rollback point and change summary.

## Model-specific delegation

- **GLM 5.2 Cloud:** large-context requirement digestion, alternative lists, research-note normalization, backlog drafts.
- **MiniMax M3 Cloud:** visual asset comparison, screenshot tagging, dialogue variations, room and enemy flavor variants.
- **Qwen3.8 Local:** private local indexing, repetitive JSON/CSV work, asset manifests, log clustering, test matrices.

If a cloud model is unavailable, Qwen3.8 may replace only routine summarization and classification. Ambiguous unit work returns to Terra; orchestration, contract, or integration questions return to Sol; bounded verification may move to Luna.

## Approval record

Every unit uses the status and ID rules in [docs/specs/README.md](./specs/README.md). Sol records contract approval by changing the owning spec to `Approved`; Terra cites `REQ-*` IDs in the implementation handoff; Luna cites `AC-*` IDs and evidence in the verification report; Sol alone records integration acceptance. A missing ID or status is a failed gate, not an implicit approval.

## Sources

- [OpenAI model selection](https://developers.openai.com/api/docs/models)
- [OpenAI GPT-5.6 Sol](https://developers.openai.com/api/docs/models/gpt-5.6-sol)
- [OpenAI GPT-5.6 Terra](https://developers.openai.com/api/docs/models/gpt-5.6-terra)
- [Ollama GLM 5.2 Cloud](https://ollama.com/library/glm-5.2%3Acloud)
- [Ollama MiniMax M3 Cloud](https://ollama.com/library/minimax-m3%3Acloud)
- [Ollama Qwen3.8](https://ollama.com/library/qwen3.8)
