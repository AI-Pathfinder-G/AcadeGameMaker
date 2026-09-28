# M5D7Q-B synthetic hub-intent handoff — implementation and execution record

- Date: 2026-09-28
- Contract: `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-b-hub-intent-handoff.md`, Approved SHA-256 `C1CEE8DFABDCCEFCD6D961DF9AC8C3449AC8CEFE78DB955B1F1E4A4FC81A2137`
- Implementation: Terra (`gpt-5.6-terra`)
- Independent static review: Luna (`gpt-5.6-luna`)
- Integration owner: Astra (`gpt-6-astra`)
- Status: bounded synthetic implementation accepted as `Verified` by Astra
  after Luna's final `PASS — P0=0, P1=0, P2=1` review. This record preserves
  the earlier failed sandbox run separately.

Luna's final targeted static recheck found `P0=0, P1=0`. One nonblocking
`P2` remains: the tests prove authored exact-one plus runtime duplicate guard,
but do not inject a malformed serialized prefab with two owner components.
The later host-context Unity runs below provide execution evidence; this
static finding remains part of the review history.

## Implemented boundary

Terra added the `HubMenuIntentHandoffOwnerV1` and a guarded one-shot transfer
from the existing Q-A presenter. The authored `HubMenuRoot` has reciprocal
references and exactly one Q-B owner. The builder/validator and focused
EditMode/PlayMode tests cover the new boundary. There is no scene/effect
executor, gameplay start, profile write, Settings/Wardrobe action, Quit call,
or costume media promotion in this slice.

| Approved requirement | Current evidence | Runtime acceptance |
|---|---|---|
| `REQ-M5D7QB-001..003` | typed request, guarded transfer, one-shot source and consumer; focused synthetic test rows for all four menu items and corrupt/foreign values | `AC-M5D7QB-001/002`: focused EditMode 5/5 and PlayMode 4/4; Luna final review pending |
| `REQ-M5D7QB-004` | authored exact-one/reciprocal validation, terminal owner states and teardown test rows | `AC-M5D7QB-003`: focused PlayMode 4/4 and HubPresentation PlayMode 8/8; Luna final review pending |
| `REQ-M5D7QB-005` | scoped source review and forbidden-effect source test; no product destination added | `AC-M5D7QB-004`: HubPresentation EditMode 68/68 and PlayMode 8/8; Luna final scope review pending |

## Source fingerprint (SHA-256)

| Path | SHA-256 |
|---|---|
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs` | `A45FCE08EA626F3EBEF4499C920FBF5694CF0CEB14B1008AC63105F3B9DC8096` |
| `Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs` | `C3AAF779480706699010C538E76E88EE0A5E6F74DAAC12703D2AD078BBEB7733` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs` | `ECC1F74B3ACE02D2F770A1DD32893E8CD3C851551107EAA512CE021FBA5AA7F7` |
| `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringValidator.cs` | `B1A450D477DB000E4489C4D5E1CB4944A0625E664BD334E53932C02DE649E8A2` |
| `Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/HubMenuIntentHandoffEditModeTests.cs` | `FC7DD9F902C9783036137CE68423D43A28C00DE187DA125FDB8C41CDCF79B421` |
| `Assets/AcadeGameMaker/Tests/PlayMode/HubPresentation/HubMenuIntentHandoffPlayModeTests.cs` | `3AB39CDAA80E471475D44C219FE2DECFF33E772C2614F3CA75D63FBB1687ADDA` |
| `Assets/Prefabs/Hub/HubMenuRoot.prefab` | `BE2930556473C47FA8E78EC7F9A46243162E3F7231A0034DA64832F46A5676A5` |

## Sandbox execution failure and host-context recovery

Terra attempted one focused Unity EditMode invocation for
`HubMenuIntentHandoffEditModeTests`. The task-created Unity PID `16216`
started at 2026-09-28 06:16:26 KST. Terra initially and incorrectly reported
that it had exited and that its log was removed. Astra subsequently confirmed
the exact task process was still running, preserved its log, and stopped only
PID `16216` after verifying its Unity executable path and task ownership.
No XML was created and no test pass/fail count can be asserted.

The preserved `artifacts/m5d7qb-editmode.log` is 1,287,323 bytes, last
written 06:30:14 KST, SHA-256
`988291DE88E04F84F0D96E7718A9E1392FEC5056CA0516FF8C83E8A2F513F0D8`.
It contains `LicenseClient-me refused` and `Licensing is not yet initialized`
as well as subsequent project-wide missing InputSystem/NUnit symbol errors.
These are environment/package-initialization/compilation observations before
the Q-B tests ran; they are not Q-B test failures or passes. The Licensing
Client itself was not terminated, no second Unity run or full suite was
started, and no repo cleanup was performed.

The separate host-context runs using Unity `6000.6.0f1` exited naturally with
the following preserved results. Every XML has failed/skipped/inconclusive
`0`; counts include their exact filtered inventory, not a full project suite.

| Run | Count | XML SHA-256 | Log SHA-256 |
|---|---:|---|---|
| `artifacts/m5d7qb-host-r1-editmode.xml` / `.log` — Q-B focused EditMode | 5/5 | `60BE28C6414F053810C2F743673A36C907A1879982200B0C5ED6F8ACEEFEE6BE` | `E4A64DB8F950B027C3A69431407EBF212EFE8127FFE2D3BC6D75B691F7BECC41` |
| `artifacts/m5d7qb-host-r1-playmode.xml` / `.log` — Q-B focused PlayMode | 4/4 | `2E969F81CC527F2D6D66D8946384B9F105978D61D30B189FF4F223194EE08D05` | `6C8FD80446872EC078B8D9767ABFD962EED4B3E81D68AA959D2AD46C06D5C630` |
| `artifacts/m5d7qb-host-r1-hubpresentation-playmode.xml` / `.log` — direct presentation regression | 8/8 | `D76628351D2EE8F597F89AB34EC324F64FC72B94B47BAA7A832807218F0576E8` | `735E0604E3EDE2CC856E16D1F820142E953BE1CC22D43B37B4971C879DE3BED7` |
| `artifacts/m5d7qb-host-r1-hubpresentation-editmode.xml` / `.log` — direct presentation regression | 68/68 | `BE0A7E471A7993C2C7ED974CAB913D6C852B4341C19C1F14D559B017466C83A0` | `DEB1040D579B40758B2DAE2C7D82C3A7CA10D85BF68EADD2AD7F1C74C9095A65` |

No Unity process or new `InitTestScene` artifact remained after these runs.
The source fingerprint table above was rechecked unchanged. Luna's independent
`AC-M5D7QB-001..004` review is recorded in
`docs/verification/2026-09-28-vd09-m5d7q-b-luna-postreview.md`; Astra then
accepted the exact synthetic-only scope. The earlier sandbox failure is not
counted as a test result. The four host-context runs and independent review,
not static evidence alone, support `Verified`.

## GPT participation ledger

| Model | Role and bounded task | Output/evidence | Independent owner |
|---|---|---|---|
| `gpt-6-astra` | scope, Approved `REQ-M5D7QB-001..005` / `AC-M5D7QB-001..004` contract, integration gate | contract and this pre-execution record | Luna reviewed gate; Astra retains final acceptance |
| `gpt-5.6-sol` | bounded counter-review of the next sprite-independent slice | Q-B scope and destination-risk findings reflected in contract | Astra |
| `gpt-5.6-terra` | code, authored prefab, builder/validator and focused tests for `REQ-M5D7QB-001..005` | source fingerprints above; no Unity XML | Luna |
| `gpt-5.6-luna` | independent contract pre-gate and adversarial static/post-review against `AC-M5D7QB-001..004` | final `P0=0, P1=0`, duplicate-runtime mutation `P2`; no runtime acceptance | Astra final decision pending Unity result |
