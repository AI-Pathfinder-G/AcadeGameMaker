# Agent Operating Rules

## Current authority — 2026-09-29 user direction / ADR-0037

This section is the active staffing and approval model. ADR-0027 preserves historical approvals and artifacts; it does not preserve superseded delegation rules. Documentation authority, Approved-only implementation, REQ/AC traceability and behavioral contracts remain binding.

- Astra (`gpt-6-astra`) owns orchestration, allocation, global design, interfaces, canon, contract approval, conflict resolution and final integration.
- Sol (`gpt-6-sol`) supplies bounded complex design, contract drafts and architectural counter-review; it has no competing final approval authority.
- Terra (implementation role, dispatched to `gpt-6-sol`) owns subsystem/content implementation, implementation tests, fixtures, validators, replay/Editor tools and QA program implementation. The current tool exposes no `gpt-6-terra`; do not invent that model ID.
- Luna (`gpt-6-luna`) owns pre/post adversarial QA, independent code/narrative review, regression/build verification and evidence digests. A worker cannot independently accept its own implementation.
- No GPT-5.6 model may be invoked for new work, retries or fallbacks. Existing GPT-5.6 agents are historical and must not be resumed. Use fresh agents with the explicit GPT-6 model assignments above. Historical evidence retains the actual models used.
- Astra/Sol/Terra/Luna are the only models used for future project work. No Ollama, Kimi, GLM, MiniMax or Qwen calls, probes, retries, schedules or paid API routes are authorized. Historical records remain factual history and are not rewritten as current assignments.
- Spark is preferred for bounded verification when actually exposed by the client/tool. This session's subagent model enum does not expose Spark; do not invent IDs or claim execution. Luna is the allowed fallback. Start with local deterministic checks to minimize GPT tokens; batch external tasks once, request short findings only, and stop on quota/auth/model errors rather than looping.
- Default main: Astra Medium standard. Pro escalation is conditional on actual tool/client support. Current subagent controls expose model and effort, NOT Pro mode; never claim higher effort is Pro or invent Pro model IDs. No separately billed API route is authorized by this change.
- Astra approves bounded contracts before Terra implements; Luna verifies independently; Astra integrates. Sol joins difficult bounded design as needed, not as a mandatory serial step.
- Historical Sol approvals remain valid. Future escalation/approval in legacy contracts now means Astra. New evidence records actual GPT participation and distinct implementation/verification ownership in the GPT participation ledger; it does not create an Ollama utilization requirement.

Read [docs/README.md](./docs/README.md), [CONTEXT.md](./CONTEXT.md), the applicable ADRs in `docs/adr/`, the applicable approved specs in `docs/specs/`, and [docs/agent-operating-model.md](./docs/agent-operating-model.md) before working.

## Documentation authority

- `CONTEXT.md` owns project vocabulary only; it must not contain implementation requirements.
- `docs/canon/` owns the current game, narrative, and art truth.
- `docs/adr/` records consequential decisions and their rationale; superseded ADRs remain as history.
- `docs/specs/` owns normative, testable behavior. Implementation may begin only for a spec whose status is `Approved`.
- `wiki/` is a publication and navigation view. It never overrides canon, ADRs, or specs.
- Every implementation change must cite requirement IDs and every verification result must cite acceptance-criterion IDs.
