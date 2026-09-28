# Kimi K3 VD-02 M1 Pure Transfer Draft Review

- Date: 2026-08-25
- Draft source: Ollama Cloud `kimi-k3:cloud`, bounded non-secret M1 prompt
- Screening authority: Sol
- Status: Screened; code rejected, limited decomposition retained
- Requirements: `REQ-WT-001~008`, `REQ-MOV-004`
- Acceptance criteria: draft input toward `AC-WT-001~006`

## Accepted input

- Separate shared aim payloads, transfer payloads, selector, session, ID validation, distance-key helper, and NUnit fixtures.
- Keep modifier application behind an engine-free target-owner sink.
- Decompose tests across ID validation, distance boundaries, mouse/gamepad ranking, failure precedence, session lifecycle, modifier rejection, and replay determinism.

## Rejected implementation

- The draft replaced frozen integer payloads with floats, world/screen positions, timestamps, and aspect fields, and changed enum/member names.
- It implemented Q1000 distance from float inputs despite the contract requiring signed-64 integer intermediates.
- It used reference classes, regex and `System.Collections.Immutable` object dictionaries where frozen immutable value payloads and exact modifier fields/IDs are required.
- It invented public helpers, event queues, profile dictionaries, and lock-age semantics outside the frozen API.
- It omitted required sink clear/idempotence, target binding, lifecycle priority, exact revision/event payloads, 21-tick recall boundary, and atomic failure behavior.
- Its mouse inside-rank direction and gamepad angle types did not match the approved ordering and integer keys.

## Sol integration decision

No Kimi code is accepted. Terra may retain only the file/test decomposition listed above and must implement directly from `docs/specs/work-contracts/2026-08-25-vd02-weight-transfer.md`. Any ambiguity returns to Sol; M2 remains blocked.
