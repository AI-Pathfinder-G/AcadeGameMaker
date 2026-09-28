# ADR-0032: GPT Terra/Luna 서브에이전트 표준

- Status: accepted
- Date: 2026-09-15
- Decision owner: Astra
- Supersedes: ADR-0031의 활성 외부 모델 라우팅 예외만 supersede한다. 과거 ADR·제안·검증 기록의 사실은 변경하지 않는다.

## Context

프로젝트 문서에는 과거 Ollama Cloud 모델(Kimi, GLM, MiniMax, Qwen)을 사용하거나 검토했던 배정·감사 기록과, 이후의 제한적 예외 규칙이 함께 남아 있다. 현재 프로젝트 작업에서는 해당 모델을 더 이상 사용하지 않으며, 활성 운영 문서가 과거 기록을 현재 작업 지시로 오해하게 만들지 않아야 한다.

## Decision

미래의 프로젝트 구현·콘텐츠·도구·QA 위임은 GPT 역할 체계로만 수행한다.

| Role | Model | Responsibility |
|---|---|---|
| Orchestrator/integrator | Astra (`gpt-6-astra`) | allocation, global design, interfaces, canon and contract approval, conflict resolution, final integration |
| Implementation worker | Terra (`gpt-5.6-terra`) | bounded subsystem/content implementation, implementation tests, fixtures, validators, replay/Editor tools, QA-program implementation |
| Independent verifier | Luna (`gpt-5.6-luna`) | pre/post adversarial QA, independent code/narrative review, regression/build verification, evidence digests |
| Optional design reviewer | Sol (`gpt-5.6-sol`) | bounded complex design, contract drafts, architectural counter-review |

Sol is optional and is not a mandatory serial gate. Terra cannot accept its own implementation. Astra alone records final integration acceptance.

## Scope and exclusions

- No future Ollama, Kimi, GLM, MiniMax or Qwen calls, probes, retries, schedules or quota-polling automations.
- No separately billed or otherwise unapproved paid API route.
- Spark may be used only if it is actually exposed by the client; no model ID is to be invented. Luna is the fallback for independent verification.
- Historical ADRs, proposals, verification reports and evidence retain their original model names and participation facts. They are not active assignments and are not rewritten as GPT participation.
- New verification evidence records the actual GPT model, role, bounded task, requirement/acceptance-criterion references, output/evidence reference and independent verifier in a GPT participation ledger.

## Operating transition

1. Active operating instructions, indexes, QA plans, verification procedures and traceability pages use this ADR and the Astra → Terra → Luna → Astra flow.
2. Any legacy contract that mentions future approval or escalation resolves to Astra; its behavioral requirements remain binding unless a newer approved decision changes them.
3. Historical external-model sections are labeled historical where they appear in active navigation or planning documents. Historical files are preserved in place.
4. Before accepting a change, Astra checks that no new task prompt, script, schedule or evidence entry introduces an Ollama or other retired-model call.

## Audit criteria

- `AGENTS.md` and `docs/agent-operating-model.md` contain no active exception permitting retired models.
- New work contracts and verification reports identify Terra implementation ownership and Luna independent verification ownership where applicable.
- Repository searches show no executable/configuration Ollama endpoint or retired-model invocation introduced by current work.
- Remaining retired-model names are confined to historical records or unrelated local media-generation documentation and are not current delegation instructions.

## Consequences

The project has a single current delegation standard, with clear implementation/verification separation and no dependency on Ollama Cloud availability. Historical evidence remains auditable, but it cannot be mistaken for a current route. When a GPT subagent quota or authentication error occurs, the task stops and is reported; it is not silently rerouted to a retired model or retried in a loop.
