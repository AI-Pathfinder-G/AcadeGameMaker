# VD-09 M5D7Q-A authored hub shell — Luna 독립 사전검토

- 검토일: 2026-09-23
- 검토자: Luna (`gpt-5.6-luna`), 독립 계약 사전검토
- 대상 계약: [M5D7Q-A authored hub shell](../specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md)
- 비교 문서: [M5D7Q 제안](../proposals/2026-09-23-vd09-m5d7q-authored-hub-presentation.md),
  [AST-UI-FONT-001](../assets/evidence/AST-UI-FONT-001.md),
  Verified M5D7O/M5D7P-A/M5D7P-B/M5D7Q0/M5D7N 계약, `AGENTS.md`,
  `docs/README.md`, `CONTEXT.md`, `docs/agent-operating-model.md`, ADR-0016,
  ADR-0027, ADR-0032
- 범위: 읽기 전용 REQ/AC·경계·소유권·폰트·allowlist·증거 계획 검토
- 실행: Unity/테스트를 실행하지 않았고 폰트 바이너리·Unity 자산·기존 파일을 만들거나 수정하지 않았다.

## Verdict

**CONDITIONAL — P0=0, P1=2, P2=2.** 현재 Draft를 `Approved`로 전환하면 안 된다.
아래 두 P1을 계약 문구와 AC에 반영한 뒤, Astra가 수정 revision을 지정하고 Luna가
좁은 범위로 재검토해야 한다. 사용자 제품 결정은 남아 있지 않다.

## 확인된 계약 완결성과 기존 경계

- 계약은 `REQ-M5D7QA-001..010`, `AC-M5D7QA-001..010` 및 각 REQ→AC traceability를
  모두 갖춘다(계약 §Requirements/§Acceptance criteria). 제안의 10개 후보 요구사항과
  9개 후보 AC를 Q-A 계약의 10개 정규 AC로 확장했으며, builder/fresh-process,
  corruption/teardown, 아래 실행 순서까지 보강했다.
- M5D7O Verified의 `Continue/New Game/Settings/Quit`, Primary/Previous 대
  Default 판정, `TopRight`, Click/Submit-only 알림 닫기 및 one-shot intent를
  그대로 투영한다. M5D7P-A Verified의 identity-bound cursor와 fixed commit 이후
  연속 소비, M5D7Q0 Verified의 단일 Q0 router/UIOnly map, M5D7N Verified의 단 한 번
  알림 take/상관성 소유권을 침범하지 않는다.
- presenter 하나만 cursor/controller/frame interpreter를 소유하고, second router,
  input module, virtual mouse, render-Update polling/queue와 모든 effect executor를
  금지한다. intent는 보존만 하며 실행하지 않고, Q0 prefab/source·map/action-owner를
  수정하지 않는 경계가 명시되어 있다.

## P1 findings

### P1-001 — full-output `OutputBackdrop`가 “safe frame 밖의 Graphic 금지”와 충돌

- **영향 ID:** `REQ-M5D7QA-001`, `REQ-M5D7QA-005`, `REQ-M5D7QA-006`,
  `AC-M5D7QA-001`, `AC-M5D7QA-007`; 상위 `REQ-UX-014`.
- 계약의 정확한 prefab topology는 `OutputBackdrop`를 full output `Image`로 요구한다
  (계약 §Exact authored assets). 바로 뒤에서는 “No Graphic or hit rectangle extends
  outside the safe frame”라고 규정한다. full-output `Image`는 non-interactive라서
  pointer quarantine에는 안전하지만, 문장 그대로는 safe frame 밖의 Graphic이다.
- 이 모순은 동일 topology와 동일 validator가 동시에 PASS할 수 없게 만든다. “interactive
  Graphic/hit rectangle만 safe frame 안”으로 제한할지, backdrop을 safe-frame 내부로
  줄일지 Astra가 선택해야 한다.
- **필수 수정:** 한 가지를 normative rule로 고정하고 `AC-M5D7QA-001/007` 및
  validator 항목에 `raycastTarget=false`, safe-frame 외부 입력 불가를 포함한다.

### P1-002 — 승인 전 font gate가 사후 생성 Unity 해시를 동시에 요구

- **영향 ID:** `REQ-M5D7QA-007`, `REQ-M5D7QA-009`, `AC-M5D7QA-008`.
- 계약 Approval gate는 승인 전에 source/license/hash/glyph/atlas 자료와 “all final
  Unity asset hashes”를 요구하는 것으로 읽힌다. 그러나 같은 계약은 generated Unity
  hashes를 Terra 구현 후 `AC-M5D7QA-008` 입력으로 추가하며 pre-approval artifact가
  아니라고 명시한다(계약 §Korean copy and static TMP font).
- AST-UI-FONT-001은 이 후행 경계를 명확히 기록한다: source archive, Regular/Bold
  OTF 및 OFL SHA-256, URL/취득일/라이선스/138 glyph/Static 90·padding 9·SDFAA·
  1024×1024·single atlas 설정은 검증됐지만 저장소 OTF/SDF/GUID/hash는 구현 후다.
  따라서 현재 자료는 source/license gate를 충족하지만 “final Unity asset hashes”의
  승인 전 요구는 충족할 수 없다.
- **필수 수정:** 승인 gate는 OTF/archive/OFL/glyph/settings 증거까지만 요구하고,
  저장소 OTF/OFL copy hash, `.meta` GUID, SDF asset/atlas/material hash는
  `AC-M5D7QA-008`의 Terra 구현 및 Luna post-review gate로 명시적으로 이동한다.

## P2 residuals (비차단, 구현 전 명시 권고)

### P2-001 — OS cursor visibility/lock은 규범 문구만 있고 독립 AC assertion이 없다

계약은 ordinary OS cursor가 visible/unlocked이고 gamepad cursor를 만들지 않는다고
규정하지만, `AC-M5D7QA-004/007`의 현재 input/layout assertion에는
`Cursor.visible`/`Cursor.lockState` 또는 동등한 증거 항목이 없다. Q0 경계를 침범하지
않는 presentation test/static audit assertion을 추가하면 UIOnly의 기존 UX 계약
(`docs/specs/vertical-demo/07-input-ui-and-feedback.md`)을 직접 증명할 수 있다.

### P2-002 — resolution PNG 파일명/manifest schema가 “named captures”로만 남아 있다

allowlist의 persistent result subtree와 test-run stems는 정확히 열거되어 있으나,
`resolution-captures/` 아래의 PNG 이름 및 assertion-manifest 필드가 명시되지 않았다.
stop 조건이 unnamed file을 금지하므로, Approved revision에서 각 필수 해상도/resize/
narrow/ultrawide/below-minimum 캡처 stem과 manifest의 viewport, integer scale,
safe rect, hit/quarantine assertions를 열거해야 한다. 이는 구현 범위를 넓히지 않는
증거 명확화다.

## Font and feasibility check

- AST-UI-FONT-001의 출처는 고정 release `Sans2.004`, 한국어 Regular/Bold OTF와
  exact OFL 1.1 text hash를 제시하며 moving/latest URL·variable/substituted font를
  금지한다. OFL 원문을 `Assets/UI/Fonts/Hub/OFL-1.1.txt`에 보존해야 한다.
- 고정 문자열(`계속하기`, `새 게임`, `설정`, `종료`, 세 알림 문구, `×`)과
  printable ASCII `U+0020–U+007E`의 합집합을 독립 계산한 결과는 정확히 **138
  code points / 42 Hangul syllables**이며 AST 목록과 일치한다. 따라서 glyph inventory
  자체는 완결됐다.
- Static population, point size 90, padding 9, SDFAA, 1024×1024, multi-atlas=false,
  no fallback/runtime addition은 Unity/TMP에서 재현 가능한 고정 설정으로 보인다.
  다만 실제 atlas가 한 장에 수용되는지, source GUID/material/sub-asset 및 fresh
  process serialization byte가 동일한지는 생성 전에는 증명할 수 없다. 이 결과는
  P1-002의 수정 후 `AC-M5D7QA-008`에서 반드시 실패/중단 가능한 preflight와
  same/fresh-process hash evidence로 닫아야 한다.

## Allowlist and evidence gate

- allowlist는 Hub presentation runtime/authoring/tests, one Input friend line, Hub
  prefab/scene, fixed font paths/license/register/evidence, and named result subtree로
  제한된다. Q0/InputRouter source, generated input, packages/project settings,
  gameplay/profile/costume/camera/narrative/effect files는 명시적으로 제외된다.
- required sequence는 font/hash baseline → package/compile/static scope → builder
  same/fresh process → real-Q0 PlayMode input/notice/intent/layout/failure → direct
  M5D7O/P-A/Q0/N regressions → full EditMode/PlayMode → Luna AC mapping 순으로 되어
  있어 계약 증거 경로가 추적 가능하다. 스크린샷만으로 input/state를 증명하지 못하고
  layout assertion과 결합해야 한다는 guardrail도 적절하다.
- 계약 status는 `Draft`이고, `AGENTS.md`/운영 모델의 “status가 Approved가 아니면
  구현 금지, Astra만 approval/integration” 규칙과 일치한다. 따라서 현재 working tree의
  부재한 Hub/font assets는 결함이 아니라 의도된 pre-implementation 상태다.

## AC disposition

| Area | Pre-gate disposition |
|---|---|
| REQ/AC IDs and traceability | PASS by contract; all 10×10 IDs and rows present |
| M5D7O/P-A/Q0/N boundary | PASS by contract; single owner, fixed-step, typed notice/correlation, no effect/Q0 mutation preserved |
| Korean copy / 138 glyph inventory | PASS by evidence and independent count; generated Unity artifacts pending implementation |
| Package/font/atlas reproducibility | PASS as planned gate, pending actual Unity generation and hashes |
| Allowlist / test sequence | PASS by contract, with P2-002 filename/manifest clarification |
| Approval readiness | **BLOCKED by P1-001 and P1-002** |

## Required next actions

1. Astra resolves P1-001 with one explicit backdrop/safe-frame rule and AC/validator
   wording.
2. Astra resolves P1-002 by separating pre-approval source/license evidence from
   post-implementation Unity asset hashes.
3. Add the P2 assertions/stems before Terra starts, then request narrow Luna re-read.
4. Keep all font binaries, SDF assets, Unity scene/prefab, and runtime implementation
   absent until status is `Approved`.

**Final recommendation:** no user decision is needed; do not advance this Draft to
`Approved` until P1-001/P1-002 are closed. After amendment, Terra may implement only the
allowlist; Luna must independently map every `AC-M5D7QA-*`, and Astra alone may accept
integration.

## Amendment re-review — 2026-09-23

- 재검토 범위: Astra가 반영한 P1-001/P1-002 및 P2-001/P2-002의 계약·제안 문구,
  AC, allowlist 증거 규칙만 좁게 재확인했다.
- 확인한 계약 SHA-256:
  `216B926768700A16596C5FC176C06A3A476831AD5E881F86DA4BDB9DC8F8DF5C`
- 확인한 제안 SHA-256:
  `5951847BAD464FD82C666DBAF33888CDBE121EE894F717809905088EE18A8165`
- Unity/테스트를 실행하지 않았고 폰트 바이너리·SDF·scene/prefab을 만들거나 기존
  파일을 수정하지 않았다. 이 재검토는 문서 게이트 판정이다.

### Finding disposition

- **P1-001 closed.** 계약은 `OutputBackdrop`을 유일한 safe-frame 외부 Graphic으로
  명시하고, `raycastTarget=false`, handler/`Selectable`/`CanvasGroup` 없음, hit-test
  제외를 고정했다. `AC-M5D7QA-001`은 이 topology를 검증하고 모든 interactive
  Graphic/hit rectangle은 safe frame 안인지 확인하며, `AC-M5D7QA-007`은
  non-interaction과 inert bars를 증명한다. 제안도 safe-frame 외부에서 허용하는
  것은 interactive Graphic이 아님을 유지한다.
- **P1-002 closed.** 승인 전에는 release/source/license/glyph/atlas 설정 증거만
  요구하고, 저장소 OTF/OFL copy hash, `.meta` GUID, SDF/atlas/material hash는
  `AC-M5D7QA-008` 및 Luna final review의 post-implementation evidence로 명시했다.
  1024×1024 단일 atlas 생성 실패는 source/설정 변경이 아닌 stop condition이다.
- **P2-001 closed.** `AC-M5D7QA-004`가 `Cursor.visible=true`,
  `Cursor.lockState=None`, gamepad-only no-cursor/no-hide/no-lock을 직접 요구한다.
- **P2-002 closed.** `resolution-captures/`의 10개 PNG 이름, `manifest.json`의
  `schemaVersion=1`, capture 순서·허용 필드, uppercase SHA-256, `assertionPassed=true`,
  resize frame 0/1 quarantine 차이가 모두 고정됐다. PNG는 layout/hit assertion을
  대체하지 않는다는 규칙도 유지된다.

### Final amended verdict

**PASS — P0=0, P1=0, P2=0. Recommend Astra advance M5D7Q-A to `Approved`.**
REQ/AC traceability, single input/cursor ownership, typed notification correlation,
effect prohibition, Q0 immutability, Korean copy/font source evidence, allowlist, and
the four prior findings are now contractually closed. This is a recommendation for
approval, not an approval-state mutation; Terra remains gated on the resulting
`Approved` status, and generated font/Unity hashes plus all AC execution evidence remain
required before final `Verified` integration.

## Latest revision recheck — resize sequence — 2026-09-23

- 최신 계약 SHA-256 확인값:
  `41983E64070E96973992738A1E2ABA61948E01176C40E57C30763ABA285CB012`
- 제안 SHA-256은 직전 검토의 확인값
  `5951847BAD464FD82C666DBAF33888CDBE121EE894F717809905088EE18A8165`와 동일하다.
- `resolution-captures` 규칙은 이제 이전에 committed 된 `640x360` viewport가
  `1280x720`으로 바뀌는 exact sequence를 명시한다. 두 resize PNG 모두 같은
  post-resize output을 사용하고, frame 0은 suppression, frame 1은 첫 stable eligible
  frame이며, 기존 normative Point/Click quarantine와 일치한다.
- manifest의 고정 파일명·순서·필드·hash/assertion 규칙 및 자동 layout/hit assertion의
  우선성도 유지되어 새 P1/P2를 만들지 않는다.

**Latest verdict 유지: PASS — P0=0, P1=0, P2=0.** 최신 revision도 `Approved` 전환
추천 상태이며, 실제 Unity/font 생성·테스트 실행 전까지는 계약 사전승인 판단만 유효하다.

## TMP Settings amendment re-gate — 2026-09-23

- 재검토 범위: Review 상태의 최신 TMP Settings amendment와 Terra의 실제
  `builder-pass-a` 실패, uGUI 2.6.0 `TMP_Settings`/`TMP_Text` package source만
  대조했다. Unity 자산·폰트·코드·계약은 수정하지 않았다.
- 확인한 계약 SHA-256:
  `0647D05718AA591B4C64AF06EE76AA03DC2B9778E0026FC989033E81AA03A731`
- 확인한 amendment SHA-256:
  `694D45DBE213C45097EF7F0887E09F56B2F2BAD69C104067AFF420588BD31B18`

### 확인된 최소 선행조건과 경계

- Terra의 `builder-pass-a`는 `TMP_FontAsset.CreateFontAsset` 전에
  `TMP_Settings.get_clearDynamicDataOnBuild`에서 null로 중단됐다
  (`docs/verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md:49-62`,
  `artifacts/unity-results/m5d7qa-20260923/builder-pass-a.log:396-403`).
  따라서 정확한 `Assets/UI/Resources/TMP Settings.asset`과 `.meta`, uGUI 2.6.0
  타입/스크립트, `assetVersion=2`, `Clear Dynamic Data on Build=true`는 실제
  공개 API 경로를 만족하는 최소 생성 전제다. runtime font-feature retrieval=false,
  직접 참조하는 정적 SDF, no fallback/OS/package resource 조건도 범위와 일치한다.
- allowlist의 두 파일, AC002/AC008의 missing/duplicate/type/script/GUID/version,
  reference/path/feature/clear-dynamic/extra-Resources 변이, `Resources.Load<TMP_Settings>("TMP Settings")`
  precondition, package-version-locked editor schema inspection 및 runtime
  reflection/private-field 경계는 충분하다. 다만 아래의 line-breaking 조건이
  닫히지 않아 현재 상태를 Approved로 올릴 수 없다.

### P1-003 — null/empty line-breaking asset과 authored `Normal` wrapping의 충돌

- amendment는 `m_leadingCharacters`/`m_followingCharacters`에 해당하는
  line-breaking TextAsset을 null/empty로 고정한다
  (`docs/proposals/2026-09-23-vd09-m5d7q-a-tmp-settings-amendment.md:27-40`).
  그러나 현재 허용된 builder는 모든 `TextMeshProUGUI`를
  `textWrappingMode=Normal`으로 author한다
  (`Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationAuthoringBuilder.cs:75`).
- uGUI 2.6.0 source는 Normal wrapping에서 Hangul/CJK 문자를 만나면
  `TMP_Settings.linebreakingRules`를 호출한다
  (`Library/PackageCache/com.unity.ugui@23caec89ae27/Runtime/TMP/TMP_Text.cs:4991-5013`).
  해당 table 초기화는 null인 두 TextAsset에 `.text`를 호출한다
  (`Library/PackageCache/com.unity.ugui@23caec89ae27/Runtime/TMP/TMP_Settings.cs:568-578`).
  따라서 exact Korean labels/notifications를 실제로 레이아웃하는 경로에서
  NullReferenceException이 남는다. 이는 단순 schema 누락이 아니라 AC007의
  no-truncation/readability와 AC008의 runtime determinism을 깨는 P1이다.
- 최소 allowlist와 고정 메시지 상자(196x64)를 함께 유지하려면 Normal wrapping을
  버리는 `NoWrap`은 충분한 해법이 아니다. 실제 알림 문자열은 29자까지라서
  그것만으로는 `AC-M5D7QA-007`의 no-truncation/readability를 보장할 수 없다.
  대신 계약/제안/validator/AC에 uGUI 2.6.0의
  `useModernHangulLineBreakingRules=true`를 canonical settings schema로
  고정·검증해야 한다. 그러면 exact Korean Hangul 문자열은 package line-breaking
  TextAsset 없이도 해당 규칙 경로를 사용한다
  (`TMP_Settings.cs:446-449`; Hangul 판정은
  `TMP_TextParsingUtilities.cs:252-260`). 이 flag를 허용하지 않거나
  Normal wrapping을 유지하면서 null 자산을 유지하려면 canonical line-breaking
  TextAsset을 추가해야 하므로 allowlist가 넓어진다.

### P2-003 — active font-feature list의 명시성

`TMP_Settings.instance`는 package source에서 `m_ActiveFontFeatures.Count`를
읽고, 새 TMP text는 그 list를 복사한다
(`TMP_Settings.cs:474-478`, `TMP_Text.cs:6248-6259`). amendment는
runtime retrieval=false를 고정하지만 이 serialized list의 non-null/empty 정책은
명시하지 않는다. canonical YAML을 만들 때 empty list(또는 package가 정한
안정된 값)를 명시하고 validator/AC008에서 null을 거부해야 한다. 이는 현재
제안의 최소 파일 수를 늘리지 않는 사전 명확화이며, line-breaking P1과 달리
정상적인 Unity-created list가 존재하면 즉시 실패하지 않으므로 P2로 분류한다.

### Amendment disposition

| Area | TMP amendment re-gate disposition |
|---|---|
| exact settings path/type/version/flags and builder precondition | PASS by source/evidence |
| allowlist, AC002/AC008 mutation coverage, no-reflection boundary | PASS in scope; add the explicit text-wrap assertion below |
| null/empty line-breaking runtime behavior | **P1-003 open** |
| active font-feature serialized-list schema | P2-003 clarification |
| user decision | **Not needed**; Astra/Sol can close this technical gate by pinning the recommended modern-Hangul rule |

**Amendment re-gate verdict: CONDITIONAL — P0=0, P1=1, P2=1.** The exact
`TMP Settings.asset` + `.meta` is the correct smallest prerequisite for the observed
builder failure, but the amendment is not yet safe to approve with null/empty
line-breaking assets and authored `Normal` wrapping. After the modern-Hangul
settings/validator assertion, plus the P2 list-schema pin, Luna can re-gate;
until then the contract must remain `Review`.

## TMP Settings amendment re-gate — latest closure — 2026-09-23

- 재검토 범위: 직전 P1-003 line-breaking 경계와 P2-003 active-feature schema의
  최신 closure, 그리고 allowlist/AC/validator 경계만 독립 확인했다. 구현·Unity
  자산·폰트 바이너리는 만들거나 수정하지 않았다.
- 최신 계약 SHA-256:
  `855AE06B96C0812B07F8A26DCC2563C12CFF3E901AC9204BE4B79AD76EFFDB01`
- 최신 amendment SHA-256:
  `CCE3C63145AFBFF02F2E244FEE52CC816D619809218470517096C59E2E81BA97`

### P1-003 closure — modern Hangul path

- 최신 amendment와 계약은 `useModernHangulLineBreakingRules=true`를
  canonical `TMP_Settings` schema와 validator mutation으로 고정하고, line-breaking
  TextAsset은 계속 null/empty로 유지한다. 이는 최소 allowlist를 넓히지 않는다.
- uGUI 2.6.0의 Normal-wrapping 조건은
  `IsHangul(charCode) && useModernHangulLineBreakingRules == false` 또는
  `IsCJK(charCode)`일 때만 `TMP_Settings.linebreakingRules`를 읽는다
  (`Library/PackageCache/com.unity.ugui@23caec89ae27/Runtime/TMP/TMP_Text.cs:4991-5013`).
  exact copy의 42 glyph는 Hangul syllable/ASCII punctuation이며, package의
  `IsCJK`에는 Hangul 범위가 없다
  (`TMP_TextParsingUtilities.cs:252-260`, `262-326`). 따라서 modern=true에서는
  Hangul 문자가 null line-breaking TextAsset 경로에 진입하지 않는다.
- `TMP_Settings.LoadLinebreakingRules`의 null `.text` 역참조 위험은 여전히
  존재하지만, 이 exact copy/modern-Hangul 조합에서는 도달하지 않는다
  (`TMP_Settings.cs:568-578`). 이는 `Normal` wrapping과 고정 196x64 notice
  box를 유지하면서 P1-003을 닫는 일관된 해법이다.

### P2-003 closure — active font-feature list

- amendment와 계약은 active font-feature list를 **non-null empty**로 고정한다.
  `TMP_Settings.instance`의 `.Count`/index migration check는 null에서 벗어나고,
  `TMP_Text.LoadDefaultSettings`의 list copy도 empty list에서 안전하다
  (`TMP_Settings.cs:474-478`, `TMP_Text.cs:6248-6259`). runtime feature
  retrieval=false도 별도로 유지된다.
- AC-M5D7QA-002와 AC-M5D7QA-008은 각각 null/non-empty list와
  `modern-Hangul=false`를 독립 mutation으로 거부하도록 명시한다. proposal은
  동일한 package-version-locked validator 검사를 요구하며, runtime reflection과
  project private-field 접근은 여전히 금지한다.

### Collateral consistency

- settings path/type/script/version/flags, exact two-file allowlist, builder의 typed
  `Resources.Load<TMP_Settings>("TMP Settings")` precondition, no-fallback/no-OS/
  no-package-resource 경계는 그대로다. 단일 입력/cursor, notice correlation,
  effect 금지, Q0 무변경, resize capture sequence에도 변경이 없다.
- 현재 Terra evidence는 여전히 settings asset 생성 전의 builder stop과 미실행
  AC를 기록한 **Blocked/not Verified** 상태다. 따라서 이번 판정은 amendment
  contract pre-gate이며, 구현 validator/runtime pass를 주장하지 않는다. Terra는
  Astra가 `Approved`를 복원한 뒤에만 새 checks를 구현하고, 이후 Luna post-gate가
  실제 AC 증거를 확인해야 한다.
- 사용자 결정은 새로 필요하지 않다.

### Latest amendment disposition

| Area | Disposition |
|---|---|
| modern Hangul path prevents null line-breaking dereference | **P1-003 closed** |
| non-null empty active-feature list and runtime copy | **P2-003 closed** |
| validator/AC002/AC008 mutation coverage | PASS by latest contract/proposal |
| allowlist and no-reflection boundary | PASS; unchanged |
| user decision | Not needed |

**Latest amendment re-gate verdict: PASS — P0=0, P1=0, P2=0.** The exact
`TMP Settings.asset` + `.meta` amendment is now internally consistent and can be
restored to `Approved` by Astra. This is approval-gate evidence only; it is not a
`Verified` implementation result, and Terra/Luna must still produce and independently
check the generated Unity assets and all AC evidence afterward.
