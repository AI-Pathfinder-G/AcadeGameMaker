# VD-09 M5D7Q-B hub intent handoff — Luna independent post-review

- Date: 2026-09-28 (Asia/Seoul)
- Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-b-hub-intent-handoff.md`
- Approved contract SHA-256: `C1CEE8DFABDCCEFCD6D961DF9AC8C3449AC8CEFE78DB955B1F1E4A4FC81A2137`
- Reviewer: Luna (`gpt-5.6-luna`)
- Implementation owner: Terra (`gpt-5.6-terra`)
- Verdict: **PASS — P0=0, P1=0, P2=1**

## Independent execution evidence

Four host-context Unity `6000.6.0f1` runs completed naturally. Every XML
reported zero failed, skipped, and inconclusive tests; no Unity process or new
temporary scene was present after the runs.

| Scope | Total / passed | XML SHA-256 | Log SHA-256 |
|---|---:|---|---|
| Focused Q-B EditMode | `5/5` | `60BE28C6414F053810C2F743673A36C907A1879982200B0C5ED6F8ACEEFEE6BE` | `E4A64DB8F950B027C3A69431407EBF212EFE8127FFE2D3BC6D75B691F7BECC41` |
| Focused Q-B PlayMode | `4/4` | `2E969F81CC527F2D6D66D8946384B9F105978D61D30B189FF4F223194EE08D05` | `6C8FD80446872EC078B8D9767ABFD962EED4B3E81D68AA959D2AD46C06D5C630` |
| HubPresentation PlayMode namespace | `8/8` | `D76628351D2EE8F597F89AB34EC324F64FC72B94B47BAA7A832807218F0576E8` | `735E0604E3EDE2CC856E16D1F820142E953BE1CC22D43B37B4971C879DE3BED7` |
| HubPresentation EditMode namespace | `68/68` | `BE0A7E471A7993C2C7ED974CAB913D6C852B4341C19C1F14D559B017466C83A0` | `DEB1040D579B40758B2DAE2C7D82C3A7CA10D85BF68EADD2AD7F1C74C9095A65` |

## Acceptance review

- `AC-M5D7QB-001`: **PASS** — the focused EditMode XML covers all four known
  menu items, exact item/receipt transfer, one-shot source transfer, and
  presenter lock preservation.
- `AC-M5D7QB-002`: **PASS** — foreign and pre-retention transfer, unknown
  intent, malformed receipt, unbound/duplicate-topology guard, proof
  corruption, and one-shot consumer rejection are covered by the focused
  suites and passed in the host runs.
- `AC-M5D7QB-003`: **PASS** — `RequestReady` to `RequestTaken`, second take,
  and `AwaitingIntent`/`RequestReady`/`RequestTaken` teardown rows passed.
- `AC-M5D7QB-004`: **PASS** — authored reciprocal exact-one topology, order,
  typed request surface, reflection fail-closed checks, and forbidden-effect
  source audit passed; direct HubPresentation namespace regressions also pass.

The implementation/source fingerprints recorded in the pre-execution record
remain the reviewed inputs, and the approved allowlist is respected. The
synthetic-only boundary remains intact: no scene executor, destination,
profile mutation, gameplay start, Settings/Wardrobe effect, Quit call, CIO,
CUA, or media promotion was introduced.

## Residual note

`P2=1`: the tests prove authored exact-one plus runtime duplicate guarding but
do not inject a malformed serialized prefab containing two owner components.
This is nonblocking because `[DisallowMultipleComponent]`, builder/validator
exact-one checks, and runtime `ValidateTopology()` fail-closed guards are all
covered.

## Final disposition

**PASS. Astra may mark M5D7Q-B `Verified`** after attaching this evidence and
the four preserved XML/log pairs. No implementation correction is required.
