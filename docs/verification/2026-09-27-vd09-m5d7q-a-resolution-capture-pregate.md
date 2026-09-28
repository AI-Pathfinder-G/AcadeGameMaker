# VD-09 M5D7Q-A resolution-capture tool — Luna 독립 사전검토

- 검토일: 2026-09-27
- 검토자: Luna (`gpt-5.6-luna`)
- 대상 proposal SHA-256: `94DB21AEB7CC6976690A96A2D0E3B38A4041CEF20C25C2F52B3709E385447838`
- 대상 Review contract SHA-256: `0A034142955929081F1FDF8BE5785C40541ABEE4FA3BA77D09A76BF792E01358`
- 범위: 실제 `Hub.unity`/`HubMenuRoot` 메모리 렌더, ScreenSpaceCamera/RenderTexture
  수명주기, safe-frame/scale-0, 정확한 10개 PNG·manifest·digest, TMP
  overflow/material/shader assertions, same/fresh 결정성, scene 복구, pair-atomic
  publish, fixed-meta GUID, result allowlist, REQ/AC/rollback/stop coverage
- 실행·구현: 실행하지 않음. generator와 capture set은 아직 부재하며, 기존
  authored scene/prefab/runtime 코드는 수정하지 않았다.

## 확인한 현재 상태와 기준선

현재 구현 표면에는 제안된
`Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
및 `.meta`가 없고, `artifacts/unity-results/m5d7qa-20260923/`에도
`resolution-capture.log` 또는 `resolution-captures/`가 없다. 이는 이 Review
단위의 구현 전 상태이며, capture 도구 부재 자체를 구현 실패로 판정하지 않는다.

기존 XML은 전체 EditMode `740/740 passed` (`full-editmode.xml`, SHA-256
`4E90187ACEAB51F0E1BEB32D83103627BE0656C713096EF7BA63C802F77F8434`) 및 전체
PlayMode `946/946 passed` (`full-playmode.xml`, SHA-256
`C4C1A9A2B8242F2D6BEB766720204590724E41B4794F0EC0F6F901C17B5816DC`)를
기록하지만, 이 결과에는 실제 PNG/manifest capture proof가 없다.

실제 scene은 `Hub.unity`의 `HubMenuRoot` prefab instance
(`2a268b00beb5c934bbd383beec0e941c`)와 runtime root를 포함한다. canonical
`HubMenuRoot.prefab`의 Canvas는 현재 `ScreenSpaceOverlay` (`m_RenderMode: 0`,
`m_Camera: {fileID: 0}`)이며, 파일 SHA-256은
`30FAC48B0F89619824A98FF91719866095F3D04D638AE4DD36FF91800AC3B27C`이다.

현재 safe-frame 구현은 `Screen.width`/`Screen.height`를 직접 읽어 scale과
actual rect를 계산한다 (`HubSafeFrameScalerV1.cs`, SHA-256
`ED25DD25FAE9A0D4E0F1775DB36731C209C76EED728E0C5BB5D45F21125D85BE`). 따라서
RenderTexture의 width/height만 바꾸어서는 capture target의 640/1280/1920/2560,
narrow, 또는 `639x359` viewport가 runtime 공식에 전달되지 않는다.

## 계약상 PASS인 부분

- 계약은 `REQ-M5D7QA-006/009`, `AC-M5D7QA-007/010`에 real-scene rendering,
  immutable authored assets, exact captures, same/fresh repeatability를 연결한다.
- 정확한 capture 파일 10개와 순서는 contract에 고정되어 있다:
  `640x360.png`, `1280x720.png`, `1920x1080.png`, `2560x1440.png`,
  `1366x768.png`, `3440x1440-ultrawide.png`, `720x1280-narrow.png`,
  `639x359-below-minimum.png`, `resize-quarantine-frame0.png`,
  `resize-quarantine-frame1.png`, 그리고 `manifest.json`이다.
- manifest top-level `schemaVersion=1`/`captures`와 capture별 exact field set,
  uppercase SHA-256, `assertionPassed=true`, resize frame-0 suppressed/frame-1
  first-stable 규칙이 계약에 명시되어 있다. `stateDigestSha256` preimage도
  UTF-8·LF 없음의 고정 문자열로 제안되어 있다.
- proposal은 real scene의 실제 uGUI/TMP geometry/material/shader를 사용하고,
  placeholder·OS font·screen scraping·runtime input/router/latch lifecycle 및
  private-state injection을 금지한다. PNG가 input/state를 증명하지 않으며
  PlayMode layout/hit assertions와 결합해야 한다는 경계도 정확하다.
- canonical validator 선행, source/font/settings/shader/license pre-hash,
  temp object/texture/camera `DestroyImmediate`, authored scene/prefab/font/
  settings/shader bytes 불변 검증, result allowlist, stop/rollback 방향은
  REQ/AC와 대체로 추적된다.
- fixed meta GUID `4f0211caf1394d76a959b6df9ab89477`는 현재 저장소 전체에서
  실제 충돌하지 않는다. 다만 아래 P1에서 preflight guard 부재를 별도로 지적한다.

## P1 findings

### P1-RES-001 — RenderTexture target size가 runtime safe-frame 입력과 연결되지 않음

제안은 임시 Camera/RenderTexture를 target size로 렌더하라고 하지만, 현재
`HubSafeFrameScalerV1.Refresh()`는 `Screen.width`와 `Screen.height`만 읽는다.
RT를 `640x360`, `639x359`, 또는 `3440x1440`으로 만들어도 Editor/Player의
`Screen` 값은 자동으로 그 값이 되지 않는다. 그러면 `floor(min(width/640,
height/360))`, centered safe rect, `scale=0`/suppression assertions가 실제
capture target과 다른 viewport를 기준으로 계산될 수 있다.

**필수 closure:** runtime 동작을 바꾸지 않는 capture-only viewport seam/context를
계약에 추가해 scaler와 digest/assertions가 동일한 명시적 `(outputWidth,
outputHeight)`를 사용하게 하거나, global display/window 변경을 사용한다면 그
변경·복구·side-effect-free 조건을 명시적으로 증명해야 한다. RT 크기만 설정하는
것은 충분하지 않다. 이 seam은 `Screen`을 임의 reflection으로 주입하거나
runtime lifecycle을 실행하는 방식이어서는 안 된다.

### P1-RES-002 — ScreenSpaceCamera/RT projection 및 pixel determinism이 고정되지 않음

실제 prefab Canvas는 ScreenSpaceOverlay인데 proposal은 메모리에서
ScreenSpaceCamera로 전환한다. 그러나 임시 Camera의 orthographic/projection,
pixelRect, culling mask/layer, clear flags/background, target display,
plane distance, RT `RenderTextureDescriptor`(RGBA32/sRGB/MSAA/depth/mipmap),
`ReadPixels` orientation 및 filter/anti-alias 조건이 normative하게 고정되지
않았다. 이 값들이 고정되지 않으면 같은 scene을 렌더해도 crop, camera scale,
letterbox, 색/aliasing, 또는 다른 scene object가 PNG에 섞일 수 있다.

**필수 closure:** capture-only camera와 RT descriptor의 exact values를 정하고,
Canvas 원본 render mode/camera/plane/pixel-perfect 등 모든 변경을 `finally`에서
복구하며, full-output inert backdrop과 UI layer만 렌더되는지 assertion해야 한다.

### P1-RES-003 — existing set 보존을 포함한 pair-atomic publish가 충분히 규정되지 않음

proposal은 sibling temporary directory에 PNG/manifest를 먼저 만들고 완성된 set만
게시한다고 하지만, target이 이미 존재할 때의 atomic swap protocol, crash 중간
상태, old-valid-set backup/restore, stale temporary directory 정리, publish 후
exact 10+manifest 재검증이 정의되지 않았다. 단순히 파일을 순서대로 이동하면
실패 중간에 partial capture set 또는 기존 유효 set 손실이 발생할 수 있다.

**필수 closure:** temporary set 전체 검증 후 directory-level atomic commit 또는
동등한 two-phase swap/rollback을 명시하고, 기존 valid set은 새 set 전체가
검증·게시될 때까지 건드리지 않으며, 실패·예외·재실행에서 temp/backup을
정리하고 최종 allowlist를 다시 검증해야 한다.

### P1-RES-004 — scene/editor 상태 복구 경계가 pre/post 증명까지 닫히지 않음

proposal은 current scene setup과 active scene을 기록하고 `Hub.unity`를 Single로
연다고 하지만, dirty scene, active-scene index, scene visibility/setup, active
selection, temporary Canvas/Camera/RT state, global screen/render state를
예외를 포함한 `finally`에서 복구한다는 규범과 pre/post equality evidence가
없다. `OpenSceneMode.Single`은 현재 unsaved editor 상태와 상호작용할 수 있으므로
scene byte hash만 동일해도 editor session state가 보존됐다고 볼 수 없다.

**필수 closure:** dirty flags와 `SceneSetup[]`, active scene, selection,
temporary render/global state를 capture 전 snapshot하고, 성공·실패·사용자 예외
모두에서 복구한 뒤 equality assertion을 남겨야 한다. scene/prefab Save API는
호출하지 않아야 한다.

### P1-RES-005 — fixed meta GUID의 collision preflight가 없음

현재 저장소에는 지정 GUID가 충돌하지 않지만, proposal은 GUID 문자열만 고정하고
모든 `.meta` scan, target path pre-existing content, GUID-to-path mismatch,
duplicate GUID를 생성 전에 fail-closed하는 절차를 명시하지 않는다. Unity meta
GUID collision은 새 generator가 올바른 path에 있더라도 import/reference를
오염시킬 수 있다.

**필수 closure:** write 전 repository-wide GUID scan과 exact target/meta
precondition을 수행하고, fixed GUID가 다른 path에 있거나 target bytes가
불일치하면 아무 파일도 쓰지 않고 stop해야 한다. scan 결과와 target absence/
identity를 `resolution-capture.log`에 남긴다.

### P1-RES-006 — manifest byte serialization이 same/fresh determinism에 부족함

manifest field set/order와 hash 대문자는 고정되어 있지만 JSON encoding(UTF-8
no-BOM), LF/CRLF, whitespace/indentation, escaping, number formatting 및
final newline 정책이 proposal/contract에 없다. 따라서 동일한 logical manifest가
serializer/runtime process에 따라 다른 bytes가 될 수 있고, `manifest bytes
동일` 요구를 독립적으로 증명할 수 없다.

**필수 closure:** canonical JSON writer 규칙(UTF-8 no-BOM, LF, fixed separators/
indentation, property/array order, invariant numeric formatting, escape policy,
final newline)을 명시하고, same/fresh SHA와 parsed field equality를 함께 검증해야
한다.

## P2 findings / required clarifications

### P2-RES-001 — TMP overflow/material/shader assertion의 관찰 항목을 더 구체화

계약과 proposal은 TMP overflow 부재, exact Korean copy, static canonical
font/material/shader를 요구하지만 어떤 `TMP_Text` 집합과 어떤 관찰값으로
판정할지 고정하지 않았다. 구현 시 각 authored label/notice `TMP_Text`에 대해
`ForceMeshUpdate` 후 `isTextOverflowing == false`, visible character/glyph
coverage·no missing/error marker, geometry bounds가 assigned rect 안임,
font asset Static/single/no-fallback, material atlas와 canonical Mobile SDF
shader identity/texture dimensions를 검증하고 manifest assertion에 매핑해야
한다. 이는 OS font나 placeholder를 이용하지 않는다는 규칙을 대체하지 않는다.

### P2-RES-002 — capture authoring seam과 runtime claim의 명시적 manifest 경계

정적 capture는 presenter/input/router/latch lifecycle을 실행하지 않고
`Continue enabled/focused`, `notification absent`를 authoring seam에서 그린다.
Manifest에는 이 상태가 `authoringView` assertion임을 기록하고, runtime input,
focus transition, hit/resize quarantine 결과로 오인하지 않도록 분리하는 것이
좋다. 실제 runtime 의미는 기존 PlayMode tests/AC-M5D7QA-004/006/007/010이
계속 권위여야 한다.

## REQ/AC/allowlist/rollback disposition

| 영역 | 사전검토 판정 |
|---|---|
| real scene/menu target and no-save intent | PASS by proposal; restoration proof needs P1-RES-004 closure |
| ScreenSpaceCamera/RT feasibility | **P1-RES-001/002 open** |
| exact 10 PNG/manifest/state digest field coverage | PASS in contract; canonical bytes need P1-RES-006 closure |
| TMP overflow/material/shader assertions | P2-RES-001 clarification required |
| same/fresh process and GPU-difference stop | PASS as stop condition; implementation evidence pending |
| pair-atomic temp publish/existing-set preservation | **P1-RES-003 open** |
| fixed meta GUID collision handling | **P1-RES-005 open** |
| REQ/AC traceability and result allowlist | PASS by contract; implementation path absent by design |
| rollback/stop coverage | PASS directionally; P1-RES-003/004 cleanup detail required |
| user product decision | **Not needed** |

## Final verdict

**CONDITIONAL — P0=0, P1=6, P2=2.**

The amendment is technically bounded and does not require a new user-facing product
decision, but Astra must not restore `Approved` yet. Terra must not implement until
the six P1 findings are closed in the proposal/Review contract and Luna performs a
narrow closure recheck. After closure, Astra may approve without another user product
decision; the result remains approval-gate evidence only. Terra must then execute
same-process/fresh-process captures, all 10 PNGs and manifest, real TMP/shader/layout
assertions, scene restoration proofs, and full XML-backed regressions. Luna's
post-review and Astra integration remain required for `Verified`.

Historical test result summaries and current hashes are recorded above solely as
evidence references; no result files or assets were generated by this pre-gate.

## Closure re-gate — 2026-09-27

이번 재검토는 위의 P1-RES-001..006과 P2-RES-001..002만 대상으로 최신 proposal과
Review contract의 closure 문구를 독립 대조했다. 구현·Unity 실행·PNG/manifest
생성은 하지 않았다.

- 최신 proposal SHA-256: `8DD84F02FEEADB67EE792511EDA00E298DA8A53B99349E4F80D2F81BC4708FFF`
- 최신 Review contract SHA-256: `393480979000121A35ABD2B6B77459CDB945627FD27852A9F5D10BB32F7C96D1`
- 고정 generator GUID `4f0211caf1394d76a959b6df9ab89477`: 현재 `Assets/` 아래
  **0 occurrences**. proposal/contract는 생성 전 repository-wide absence와
  생성 후 canonical path 단일 identity를 요구한다.

### P1 closure 확인

- **P1-RES-001 closed:** capture `RenderTexture` dimensions만 입력으로 삼는
  독립 integer-scale oracle을 추가했고 `Screen.width/height` 및
  `HubSafeFrameScalerV1.Refresh()`를 호출하지 않는다. 계산한 SafeFrame을 실제
  scene object에 메모리에서만 적용하며 runtime resize/input semantics는 XML
  suite가 전담한다. 따라서 capture oracle과 runtime XML 경계가 분리되어 있다.
- **P1-RES-002 closed:** Camera position/orthographic size, near/far, rect/pixelRect,
  clear/culling/targetDisplay, HDR/MSAA/occlusion/dynamic-resolution, Canvas
  ScreenSpaceCamera/plane/pixel-perfect/layer, ARGB32 depth-24 sRGB RT,
  RGBA32 readback, mip/MSAA/GL.sRGBWrite/RenderTexture.active 복구가 exact
  values로 고정됐다.
- **P1-RES-003 closed:** `resolution-captures.tmp`에 전체 set을 검증한 뒤
  기존 final을 `.prev`로 이동하고 tmp를 final로 이동한다. move 실패 시 `.prev`를
  복원하며 stale tmp/prev는 사전 fail-closed, 성공 후 prev는 제거한다. partial
  final은 수용하지 않고 기존 valid set은 완성된 새 set까지 보존한다.
- **P1-RES-004 closed:** dirty loaded scene, stale temp/prev, graphics surface
  문제를 mutation 전 거부하고, 하나의 `finally`에서 SceneSetup/active scene,
  Selection, Canvas/SafeFrame, RenderTexture.active, GL.sRGBWrite와 temporary
  objects를 복구·equality-check한다. authored dependency bytes는 before/after
  hash로 비교하며 Save API는 사용하지 않는다.
- **P1-RES-005 closed:** generator meta 생성 전에 GUID absence를 확인하고,
  생성 후 canonical path로만 resolve되는지 확인하며 다른 occurrence는
  거부한다. 현재 Assets scan은 0으로 독립 확인했다.
- **P1-RES-006 closed:** manifest는 UTF-8 no-BOM, two-space indentation,
  LF-only, invariant base-10 integers, lowercase booleans, exact field/array
  order, no optional/trailing spaces, exactly one final LF로 canonicalize된다.

### P2 closure 확인

- **P2-RES-001 closed:** 여섯 authored TMP path의 exact copy와 Regular Static
  font, canonical Mobile SDF material/shader, Normal wrapping, configured size,
  no fallback/dynamic path를 검사한다. `ForceMeshUpdate` 뒤 preferred bounds,
  `isTextOverflowing=false`, non-space character record coverage를 확인한다.
- **P2-RES-002 closed:** `CONTINUE_ENABLED_FOCUSED|NOTICE_ABSENT`를 명시적
  authoring capture token으로 정의하고, runtime handoff/input/cursor/
  quarantine 실행을 주장하지 않는다. 해당 runtime 의미는 XML-backed
  PlayMode/REQ-AC 증적에만 남는다.

### Closure 판정

| 항목 | 결과 |
|---|---|
| P0 | 0 |
| P1-RES-001..006 | **CLOSED** |
| P2-RES-001..002 | **CLOSED** |
| 현재 resolution-capture pre-gate | **PASS** |

**최종 closure verdict: PASS — P0=0, P1=0, P2=0.** 새 사용자 제품 결정은
필요 없다. Astra는 최신 contract를 `Review → Approved`로 복원할 수 있으나,
이는 구현 성공이나 `Verified`를 선취하지 않는다. Approved 복원 후 Terra가
same/fresh capture 실행·10개 PNG와 manifest·XML 경계·복구/allowlist 증적을
제출하고, Luna post-review와 Astra 통합 승인을 거쳐야 한다.
