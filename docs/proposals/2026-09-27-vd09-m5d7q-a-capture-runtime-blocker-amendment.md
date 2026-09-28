# M5D7Q-A 캡처 실제 실행 blocker 보정 제안

- 날짜: 2026-09-27
- 설계·반대검토: Sol (`gpt-5.6-sol`)
- 승인 책임: Astra
- 상태: 제안 — 구현 권한 또는 기존 계약 승인 변경이 아님
- 대상: `REQ-M5D7QA-006/008/009`, `AC-M5D7QA-007/010`
- 최초 검토한 retry-e generator SHA-256:
  `5B26C2D99BEC3C4A6984848532271AC60E098DEBB1F7B9E6516930A3E08940AE`
- retry-f/retry-g 실행 뒤 현재 관찰한 generator SHA-256:
  `B499A12F1C8AA2E04CF330D6B3DBB557257ABA706E6DC8133A148ABF6AD96214`
- 직접 blocker 증거:
  `artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-e.log` 및
  `resolution-capture-retry-g.log`

## 결론

`resolution-capture-retry-e.log`는 PNG를 하나도 만들기 전에 서로 독립인 두
실행 blocker를 함께 드러냈다.

1. `SafeFrame/MenuPanel/Continue/Label`의 TMP 단언은 preferred size,
   `textBounds`, overflow flag, record count를 한 boolean과 한 메시지로 합쳐 어느
   항이 실패했는지 알 수 없다. 특히 TMP의 preferred size와 `textBounds`는 실제
   렌더 quad의 RectTransform-local 경계와 같은 측정치가 아니다.
2. batchmode가 빈 `SceneSetup[]`로 시작한 뒤 `Hub.unity`가 마지막 loaded scene이
   되면 Unity 6000.6은 그 scene을 닫지 않는다. 따라서 같은 process 안에서
   zero-loaded-scene 상태를 복원했다고 주장할 수 없다.

보정은 제품 또는 runtime 동작을 바꾸지 않는다. TMP는 실제 local-space mesh
quad, RectTransform rect, TMP overflow 상태, exact glyph record를 각각 검사한다.
빈 초기 setup은 exact terminal batchmode process에서만 허용하고, authored bytes를
저장하지 않은 채 process 종료를 격리 경계로 사용한다. 이 경우 scene setup을
복원했다는 주장은 하지 않는다.

## Retry-e와 retry-g가 증명한 범위

- Unity `6000.6.0f1`, D3D12, non-Null graphics device에서 exact entrypoint가
  실행되었다.
- 첫 capture의 render/readback/PNG write 전에
  `TMP bounds malformed: SafeFrame/MenuPanel/Continue/Label`로 중단되었다.
- 현재 메시지로는 `preferredWidth`, `preferredHeight`, `textBounds`,
  `isTextOverflowing`, `characterCount` 중 어느 항이 실패했는지 판별할 수 없다.
- 정리 중 `EditorSceneManager.CloseScene(hub, true)`가 마지막 loaded Hub scene을
  닫지 못했고, generator가 `Cannot close capture scene for empty setup.`을 추가했다.
- 최종 결과는 exit code `1`이고 `resolution-captures/`가 게시되지 않았다. 이
  로그는 실패 증거이며 수용 capture 증거가 아니다.

추가 실제 실행인 `resolution-capture-retry-g.log`는 Hub나 tmp를 만들기 전
`Terminal empty-session setup precondition failed.`로 중단되었다. 이 process도
`GetSceneManagerSetup()`은 empty였지만, warmed Unity startup은 transient한 clean
unsaved bootstrap scene 하나를 loaded/active 상태로 둘 수 있다. 따라서
`setupCount == 0`에서 `sceneCount == 0 && active invalid`만 허용한 것은 Unity
6000.6의 실제 시작 상태보다 좁았다. retry-g는 이 상태의 exact 값을 기록하지
않았으므로 성공 증거가 아니며, 다음 실행은 predicate 평가 전에 진단 marker를
남겨야 한다.

## 보정 1 — TMP 진단과 authoritative no-overflow predicate

### 측정 순서

각 target에 layout oracle을 적용한 뒤 다음 순서를 지킨다.

1. `Canvas.ForceUpdateCanvases()`를 호출한다.
2. exact six TMP components 각각에
   `ForceMeshUpdate(ignoreActiveState: true, forceTextReparsing: true)`를 호출한다.
3. 다시 `Canvas.ForceUpdateCanvases()`를 호출한다.
4. 아래 observation을 한 번 수집한다. observation을 수집하는 도중 text, font,
   material, rect, margin 또는 scene을 변경하지 않는다.
5. 진단 한 줄을 먼저 기록하고, 그 다음 predicate를 평가한다. 따라서 실패해도
   어느 값이 실패했는지 로그에 남는다.

### exact diagnostic line

로그는 TMP마다 UTF-8 한 줄을 다음 고정 field 순서로 기록한다. 숫자는
`InvariantCulture`와 round-trip 형식(`R`)을 사용하고 boolean은 lowercase다.
copy는 UTF-16 code unit을 `U+XXXX` 목록으로 기록해 줄바꿈·locale 영향을 없앤다.

```text
M5D7QA_TMP_OBSERVATION|path=<path>|copy=<U+XXXX,...>|rect=<xmin,ymin,xmax,ymax>|margin=<l,t,r,b>|preferred=<w,h>|textBounds=<xmin,ymin,xmax,ymax>|meshBounds=<empty|xmin,ymin,xmax,ymax>|overflow=<true|false>|firstOverflow=<int>|characterCount=<int>|visibleCount=<int>|nonSpaceCount=<int>|lineCount=<int>|materialCount=<int>|failures=<NONE|sorted comma-separated codes>
```

실패 exception은 observation을 숨기지 않고
`M5D7QA_TMP_ASSERT_FAIL|path=<path>|failures=<codes>`로 끝난다. failure code는 최소
다음을 구분한다.

- `NONFINITE`, `RECT_INVALID`, `MARGIN_DRIFT`
- `OVERFLOW_FLAG`, `OVERFLOW_INDEX`
- `CHAR_COUNT`, `CHAR_MISMATCH_<index>`, `NONSPACE_INVISIBLE_<index>`
- `FOREIGN_FONT_<index>`, `NONCHAR_ELEMENT_<index>`,
  `FOREIGN_MATERIAL_<index>`
- `MESH_INDEX_<index>`, `MESH_VERTEX_MISMATCH_<index>`,
  `VERTEX_OUTSIDE_RECT_<index>`

`preferred`와 `textBounds`는 반드시 기록하지만 단독 pass/fail 조건으로 사용하지
않는다. 이는 진단 삭제가 아니라 측정 의미의 분리다. TMP 2.6.0의 preferred 값은
font line metrics를 포함한 layout 요청 크기이고, `textBounds`는 visible character의
`origin/xAdvance/ascender/descender` 합집합이다. 둘 다 실제 SDF quad의 네 mesh
vertex가 RectTransform-local rect를 벗어났는지 직접 증명하지 않는다.

### authoritative predicate

각 component는 기존 exact path/copy/font/material/shader/size/Normal wrapping/static
atlas/fallback 금지 단언을 먼저 만족해야 한다. 추가 no-overflow predicate는 다음과
같다.

1. `Rect r = text.rectTransform.rect`를 유일한 containment 좌표계로 사용한다.
   `r.xMin`, `r.yMin`, `r.xMax`, `r.yMax`와 모든 검사 vertex는 같은 TMP object
   local space다. world/screen/canvas 좌표와 safe-frame scale을 섞지 않는다.
2. 현 canonical asset은 margin이 exact `(0,0,0,0)`이어야 한다. margin drift는
   실패한다. 따라서 containment box는 그대로 `r`이다.
3. rect와 모든 측정값은 finite이고 `r.width > 0`, `r.height > 0`이어야 한다.
4. `text.isTextOverflowing == false`이고
   `text.firstOverflowCharacterIndex == -1`이어야 한다. 어느 하나라도 다르면 실제
   overflow failure이며 preferred/mesh 결과와 관계없이 중단한다.
5. 이 계약의 exact copy는 markup 없는 BMP code unit만 사용한다. 따라서
   `textInfo.characterCount == text.text.Length`이고 각 index의
   `characterInfo[i].character == text.text[i]`여야 한다. empty Message는 양쪽 모두
   zero여야 한다.
6. non-space source code unit마다 exact one `TMP_CharacterInfo`가 있어야 하며
   `elementType == Character`, `isVisible == true`, `fontAsset`은 canonical Regular,
   `materialReferenceIndex == 0`이어야 한다. source space도 같은 index의 record를
   유지하되 visible glyph 수에는 포함하지 않는다. visible count는 exact
   non-space count와 같아야 한다.
7. visible record마다 `vertexIndex >= 0`이고
   `vertexIndex + 3 < textInfo.meshInfo[0].vertices.Length`여야 한다. 그 네 실제 mesh
   vertex는 같은 record의 `bottomLeft`, `topLeft`, `topRight`, `bottomRight`와
   component-wise exact float equality여야 한다. NaN/Infinity는 실패한다.
8. 네 vertex 각각에 대해 `r.xMin <= x <= r.xMax`와
   `r.yMin <= y <= r.yMax`를 epsilon 없이 만족해야 한다. visible record 전체의
   네 vertex 합집합이 diagnostic `meshBounds`다. non-empty copy의 meshBounds는
   non-empty여야 하고, empty Message만 empty meshBounds를 허용한다.

이 predicate는 no-overflow를 약화하지 않는다. TMP 자체 overflow 판정과 실제
렌더 mesh의 local rect containment를 동시에 요구하고, 모든 non-space source가
canonical font/material의 visible quad 하나로 존재함을 요구한다. 반대로 font의
line-metric 요청 크기만으로 실제 mesh overflow라고 오판하는 조건은 제거한다.

어떤 exact label이라도 `OVERFLOW_FLAG`, `OVERFLOW_INDEX` 또는
`VERTEX_OUTSIDE_RECT_*`로 실패하면 이 보정 범위에서 font size, copy, label rect,
wrapping, atlas 또는 scale을 바꾸지 않는다. capture는 PNG 전에 중단한다. 실제
layout 변경은 별도 Astra 계약 보정과 Luna pre-gate가 있어야 한다.

## 보정 2 — empty SceneSetup의 terminal batchmode 경계

### 두 restoration mode

초기 `SceneSetup[]` 길이에 따라 mode를 고정한다.

- `RestorableEditorSession`: setup이 하나 이상이다. 기존 계약대로 전체 setup과
  active scene을 exact 복원하고 equality를 증명한다.
- `TerminalEmptyBatchSession`: setup이 zero다. 아래 terminal precondition을 모두
  만족할 때만 허용한다. Hub는 마지막 loaded scene이므로 닫거나 zero scene으로
  복원하려 하지 않는다.

`TerminalEmptyBatchSession` precondition은 Hub를 열거나 tmp directory를 만들기
전에 모두 확인한다.

1. `Application.isBatchMode == true`.
2. command line에 독립 token `-batchmode`와 `-quit`이 각각 exact one 존재한다.
3. `-executeMethod`의 바로 다음 token이 exact
   `AcadeGameMaker.Hub.Authoring.Editor.HubPresentationCaptureGenerator.Capture`다.
4. `-runTests`가 없고 초기 `EditorSceneManager.GetSceneManagerSetup()`이 empty다.
5. 아래 preflight scene snapshot marker를 먼저 기록한 뒤 초기 scene 상태가
   `TrueZeroScene` 또는 `CleanUnsavedBootstrap` 중 exact one으로 분류된다.
6. loaded scene이 필요한 다른 Editor callback 또는 후속 작업을 이 entrypoint가
   예약하지 않는다. entrypoint는 성공 return 또는 exception 뒤 즉시 `-quit`
   process 종료로 이어지는 terminal 작업이다.

하나라도 다르면 scene을 열기 전에 실패한다. interactive Editor, `-quit` 없는
batchmode, test runner 또는 다른 executeMethod에서 empty-setup 예외를 재사용할 수
없다.

### preflight scene snapshot과 허용 분류

`setupCount == 0`을 관찰하면 scene predicate를 평가하거나 예외를 던지기 전에
다음 한 줄을 반드시 기록한다. path와 name은 JSON string escaping을 사용하고,
숫자는 invariant base-10, boolean은 lowercase다. invalid active scene에서는
handle `0`, path/name `""`, loaded/dirty `false`, build index `-1`, root count `0`을
기록한다.

```text
M5D7QA_TERMINAL_SCENE_SNAPSHOT|phase=preflight|setupCount=<n>|sceneCount=<n>|activeValid=<bool>|activeHandle=<int>|activePath="<escaped>"|activeName="<escaped>"|activeLoaded=<bool>|activeDirty=<bool>|activeBuildIndex=<int>|activeRootCount=<int>
```

그 다음 exact one 분류만 허용한다.

- `TrueZeroScene`: `sceneCount == 0`이고 active scene이 invalid다.
- `CleanUnsavedBootstrap`: `sceneCount == 1`이고 `GetSceneAt(0)`이 valid/loaded이며
  exact active scene과 같은 handle이다. `path == string.Empty`, `isDirty == false`,
  `buildIndex == -1`, root GameObject count `0`이어야 한다. scene name은 위 marker와
  snapshot에 exact 보존하지만 admission 값으로 hard-code하지 않는다. Unity의
  transient bootstrap name은 locale/startup 경로에 종속될 수 있고, empty path,
  build index, zero roots, clean/loaded/active identity가 비영속 bootstrap을
  구분하는 authoritative 조건이다.

분류 성공은 다음 marker를 남긴다.

```text
M5D7QA_TERMINAL_SCENE_CLASSIFICATION|kind=<TRUE_ZERO_SCENE|CLEAN_UNSAVED_BOOTSTRAP>|failures=NONE
```

predicate failure는
`M5D7QA_TERMINAL_SCENE_CLASSIFICATION|kind=REJECTED|failures=<sorted codes>`와 같은
codes의 exception을 남기고 어떤 scene/file/tmp도 변경하기 전에 중단한다. 최소
failure codes는 다음과 같다.

- command boundary: `NOT_BATCHMODE`, `BATCHMODE_TOKEN_COUNT`,
  `QUIT_TOKEN_COUNT`, `EXECUTE_METHOD_TOKEN_COUNT`, `EXECUTE_METHOD_MISMATCH`,
  `RUNTESTS_PRESENT`
- initial scene shape: `SCENE_COUNT_UNSUPPORTED`, `ZERO_ACTIVE_VALID`,
  `BOOTSTRAP_INVALID`, `BOOTSTRAP_NOT_LOADED`, `BOOTSTRAP_NOT_ACTIVE`,
  `BOOTSTRAP_PATH_NONEMPTY`, `BOOTSTRAP_DIRTY`, `BOOTSTRAP_BUILD_INDEX`,
  `BOOTSTRAP_ROOTS_NONZERO`

성공 분류는 immutable in-memory `InitialTerminalSceneSnapshot`으로 보유한다.
`CleanUnsavedBootstrap`은 persistent asset이 아니며 `OpenSceneMode.Single`로 Hub를
열 때 대체되어도 된다. tool은 bootstrap scene을 저장하거나 복제하거나 나중에
재생성하지 않는다.

### terminal cleanup and proof

terminal mode도 다음 상태는 기존과 동일하게 복원하고 equality를 검사한다.

- `RenderTexture.active`, `GL.sRGBWrite`, `Selection.objects`,
  `Selection.activeObject`;
- `QualitySettings.antiAliasing`, `QualitySettings.activeColorSpace`의 관찰값;
- actual Canvas의 render mode/camera/plane/pixel-perfect/channels;
- root/SafeFrame/backdrop RectTransform, notice active state, Selectable
  interactability, Image enabled state, TMP copy;
- temporary Camera, RenderTexture, Texture2D의 완전 파괴와 camera target detach.

그 후 scene setup equality 대신 다음 terminal condition을 증명한다.

- loaded scene은 canonical `Assets/Scenes/Hub.unity` exact one이고 active다.
- Hub `Scene.isDirty == false`이며 어떤 `SaveScene`, `SaveOpenScenes`,
  `AssetDatabase.SaveAssets`, `EditorUtility.SetDirty`도 호출하지 않았다.
- contract dependency의 before/after SHA-256가 모두 같다. scene/prefab/font/
  TMP settings/shader/license/generator/meta를 포함한다.
- 허용된 persistent difference는 capture output directory와 지정 log뿐이다.
- generator가 성공 return 또는 failure throw를 끝내면 `-quit` process가 종료한다.
- initial snapshot이 `TrueZeroScene`이었든 `CleanUnsavedBootstrap`이었든 초기
  scene state 복원을 시도하거나 주장하지 않는다. exact clean Hub는 process
  종료 때까지 유지되고 종료가 모든 in-memory scene state를 폐기한다.

로그는 시작과 종료에 각각 다음 marker를 남긴다.

```text
M5D7QA_TERMINAL_EMPTY_SETUP|phase=accepted|batchmode=true|quit=true|setupCount=0|initialSceneKind=<TRUE_ZERO_SCENE|CLEAN_UNSAVED_BOOTSTRAP>|policy=PROCESS_BOUNDARY_NO_SAVE
M5D7QA_TERMINAL_EMPTY_SETUP|phase=complete|initialSceneKind=<TRUE_ZERO_SCENE|CLEAN_UNSAVED_BOOTSTRAP>|hubLoaded=true|hubDirty=false|persistentHashesEqual=true|globalsRestored=true|initialSceneRestored=false|zeroSceneRestored=false
```

두 번째 marker의 `initialSceneRestored=false`와 `zeroSceneRestored=false`는 성공
조건이다. 이는 true zero scene 또는 transient bootstrap scene의 불가능하거나
무의미한 복원을 성공으로 포장하지 않고 process boundary를 명시한다.
non-terminal mode에서는 이 marker를 쓰지 않고 기존 exact setup equality를
기록한다.

### same-process repeatability under the terminal condition

zero scene으로 돌아갈 수 없으므로 terminal mode에서 public entrypoint를 두 번
호출했다고 주장하지 않는다. 대신 한 entrypoint 안에서 Hub를 한 번 연 뒤 exact
10-spec render/assert/encode pass A와 pass B를 수행한다.

- pass A의 PNG bytes와 canonical manifest bytes를 tmp set으로 만든다.
- view/oracle을 canonical authoring state로 다시 적용·단언하고 pass B를 수행한다.
- 각 PNG와 재생성 manifest를 byte-for-byte 비교한다.
- mismatch면 publish 전에 실패한다. pass B 파일은 별도 persistent 결과로 남기지
  않는다.
- equality는
  `M5D7QA_SAME_PROCESS_CAPTURE|passes=2|png=10/10|manifest=true|equal=true`로
  기록한다.

이는 같은 process·scene·GPU에서 두 independent render pass가 같은 결과를 냈다는
증거다. fresh-process 증거는 별도 Unity process에서 같은 source SHA로 실행해 기존
final set과 byte equality를 다시 요구한다.

## failure, rollback, evidence policy

1. preflight, TMP predicate, render, readback 또는 assertion 실패는 PNG publication
   전에 tmp만 제거하고 기존 final set을 건드리지 않는다.
2. staged publication 뒤 GUID/hash/global/non-terminal restoration 또는 terminal
   proof가 실패하면 새 final을 제거하고 이전 final을 `.prev -> final`로 exact
   복원한다. 이전 final이 없었다면 final을 absent로 복원한다.
3. rollback 자체가 실패하면 추가 delete/move를 중단하고 original failure와
   rollback failure를 aggregate하여 exit code `1`로 남긴다. 남은 directory를
   수용 증거로 취급하지 않는다.
4. terminal mode의 성공과 실패 모두 Hub를 저장하지 않는다. 마지막 loaded Hub를
   닫으려 하지 않고 process 종료가 in-memory scene을 폐기한다.
5. 모든 authored dependency hash mismatch, dirty Hub, missing terminal marker,
   TMP failure code, same-process byte drift, nonzero exit, missing/extra capture file,
   또는 prior-final rollback 불일치는 stop condition이다.
6. `resolution-capture-retry-e.log`, `resolution-capture-retry-f.log`,
   `resolution-capture-retry-g.log`와 이전 blocker logs/capture artifacts는
   overwrite, 삭제 또는 성공으로 재해석하지 않는다.

### 다음 exact evidence stem

Astra가 이 보정을 승인하고 Luna가 corrected source의 exact SHA를 pre-gate한 뒤 첫
실행은 새 immutable log-only stem
`artifacts/unity-results/m5d7qa-20260923/resolution-capture-retry-h.log`를 사용한다.
이 이름은 계약 result allowlist에 먼저 추가되어야 한다. 성공 조건은 exit `0`,
preflight scene snapshot과 accepted classification, six TMP observation groups의
`failures=NONE`, terminal accepted/complete markers, same-process equality marker,
complete final set, authored hash equality다.

그 다음 clean Unity process는 이미 예약된 `resolution-capture-fresh.log`를 사용한다.
두 로그는 같은 generator SHA를 기록해야 하고 fresh run은 prior final과 exact
PNG/manifest equality를 증명해야 한다. `retry-h` 하나만으로
`AC-M5D7QA-007/010` 또는 `Verified`를 주장할 수 없다. Luna post-review와 Astra
integration acceptance가 계속 필요하다.

## exact contract delta proposal

승인 시 기존 Approved 계약에서 다음 문구만 좁게 보정한다.

- “preferred bounds”를 diagnostic observation으로 정의하고 authoritative
  no-overflow를 위 local mesh/rect/overflow/glyph predicate로 정의한다.
- “restores the original Editor scene setup”을 non-empty setup에는 그대로 유지하고,
  empty setup에는 `TrueZeroScene` 또는 exact `CleanUnsavedBootstrap` snapshot을
  허용하는 terminal batchmode process-boundary/no-save/hash-equality 조건을
  적용한다. 두 terminal initial state 모두 복원을 주장하지 않는다.
- same-process 두 실행 요구를 terminal empty setup에 한해 한 entrypoint의 two
  independent render passes와 exact byte comparison으로 구체화한다.
- result allowlist에 `resolution-capture-retry-h.log`만 다음 실행 stem으로
  추가하고 retry-f/retry-g는 immutable failure evidence로 보존한다.
- implementation allowlist, runtime assembly, scene/prefab/font/material/TMP asset,
  capture filenames/manifest schema/oracle/state token은 바꾸지 않는다.

## REQ/AC traceability

| 보정 항목 | Requirement | Acceptance criterion | 필요한 증거 |
|---|---|---|---|
| local TMP diagnostic와 exact glyph/mesh containment | `REQ-M5D7QA-006` | `AC-M5D7QA-007` | six-path observation, failure-code absence, ten captures |
| overflow/failure fail-closed | `REQ-M5D7QA-008` | `AC-M5D7QA-009`, `AC-M5D7QA-010` | overflow negative mutation, pre-publication stop, no asset/output drift |
| authored immutability와 atomic publication | `REQ-M5D7QA-009` | `AC-M5D7QA-007`, `AC-M5D7QA-010` | dependency hashes, rollback probes, complete set validation |
| non-empty exact restoration | `REQ-M5D7QA-009` | `AC-M5D7QA-007` | setup/active scene equality |
| empty terminal process boundary | `REQ-M5D7QA-009` | `AC-M5D7QA-007`, `AC-M5D7QA-010` | pre-predicate scene snapshot, exact zero/bootstrap classification, no-save/clean Hub/hash proof, terminal markers, process exit |
| same/fresh determinism | `REQ-M5D7QA-006/009` | `AC-M5D7QA-007/010` | pass A/B equality in retry-h and prior-final equality in fresh log |

기존 runtime/input/notification/menu effect 경계인 `REQ-M5D7QA-001..005/007/010`의
제품 의미는 변경하지 않는다. `REQ-M5D7QA-010`의 effect 금지도 그대로다.
이 보정은 기존 REQ/AC ID 또는 traceability를 추가·삭제·재매핑하지 않는다.

## 사용자 결정 여부와 승인 gate

새 사용자 제품 결정은 필요 없다. copy, font, font size, layout, colors, interaction,
runtime behavior, capture list와 manifest schema를 바꾸지 않고 Editor-only evidence
도구의 판정과 process lifecycle을 정확히 한다.

단, 새 diagnostic이 실제 TMP overflow 또는 mesh의 rect 이탈을 증명해 이를 고치기
위해 copy/typeface/font size/visible layout을 변경하려면 이 제안은 그 변경을
승인하지 않는다. Astra가 별도 계약 delta를 만들고, 사용자 선택을 바꿀 정도의
제품 변화라면 그때 사용자 결정을 요청해야 한다.

현재 다음 단계는 Astra의 bounded contract 승인, Terra의 exact implementation,
Luna의 corrected-source pre-gate와 post-run 독립 검증 순서다.
