# Multi-Agent Operating Model

## Current override — 2026-09-29

[ADR-0037](./adr/0037-gpt6-role-model-migration.md) applies the user's explicit GPT-6 migration. Astra uses `gpt-6-astra`, Sol uses `gpt-6-sol`, Terra's implementation role uses a separate `gpt-6-sol` agent, and Luna uses `gpt-6-luna`. The current dispatch tool exposes no `gpt-6-terra`; that identifier must not be invented or reported as executed. New work, retries and fallbacks must never invoke GPT-5.6. Existing GPT-5.6 workers are not resumed. The role and implementation/independent-verification separation remain unchanged; historical evidence is preserved.

## Previous routing decision — 2026-09-15

[ADR-0032](./adr/0032-gpt-terra-luna-subagent-standard.md) supersedes ADR-0031 for active external-model routing. Future project work uses only GPT Astra, Sol, Terra and Luna. No Ollama, Kimi, GLM, MiniMax or Qwen calls, probes, retries or schedules are permitted. Spark is used only if actually exposed; otherwise Luna is the verification fallback. Historical ADRs and evidence retain their factual past records.

## Current operating model — effective 2026-09-08

[ADR-0027](./adr/0027-astra-orchestration-and-gpt-only-delivery.md) is the active staffing decision. Historical staffing, delegation and utilization rules remain only in their ADRs and evidence.

| Active model | Ownership |
|---|---|
| Astra | Orchestration, global contracts/canon, approvals and final integration |
| Sol | Bounded complex design, contract drafts and architectural counter-review |
| Terra | Implementation/content, implementation tests, fixtures, validators and QA/Editor/replay tooling |
| Luna | Pre/post adversarial QA, independent code/narrative review, regressions/build evidence |

Workflow: Astra scope -> Terra impact and Luna failure modes -> Astra Approved -> Terra implementation -> Luna independent verification -> Astra final integration. Sol joins only when difficult design warrants it; it is not a mandatory serial gate. A worker cannot independently accept its own implementation.

No Ollama assignment is active or eligible for reassignment. Future implementation and QA work is dispatched to the GPT roles above, with GPT participation and distinct implementation/verification ownership recorded in new evidence. Historical artifacts and approval dates stay intact; old behavioral contracts remain binding. Future approvals and escalations under old contracts belong to Astra.

Pro mode is distinct from reasoning effort. Use it only when execution support is confirmed; current subagent tools expose no Pro-mode control. Default Astra Medium standard, stronger reasoning for evidenced risk, no automatic paid API routing.

## Approval record

Every unit uses the status and ID rules in [docs/specs/README.md](./specs/README.md). Astra records contract approval by changing the owning spec to `Approved`; Terra cites `REQ-*` IDs in the implementation handoff; Luna cites `AC-*` IDs and evidence in the verification report; Astra alone records integration acceptance. A missing ID or status is a failed gate, not an implicit approval.

## Source

- [ADR-0027](./adr/0027-astra-orchestration-and-gpt-only-delivery.md)
