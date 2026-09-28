# M5D7Q-A 결정론적 해상도 캡처 도구 보정 제안

- 날짜: 2026-09-27
- 설계·승인 책임: Astra
- 상태: 계약 보정 제안 — 구현 권한 아님
- 대상: `REQ-M5D7QA-006/009`, `AC-M5D7QA-007/010`

## 문제와 결론

허브 생성물과 XML 테스트는 통과했지만, 승인 계약이 요구하는 열 개 PNG와
assertion manifest를 만드는 실행 경로가 allowlist에 없다. PNG를 임의 제작하거나
테스트 결과만으로 시각 증거를 대체하지 않는다. Editor-only 도구 하나를 추가해
실제 `Assets/Scenes/Hub.unity`와 그 안의 실제 `HubMenuRoot` 인스턴스를 메모리에서
열고, 저장 없이 고정 출력 크기로 렌더링한다.

새 사용자 제품 결정은 필요 없다. 색, 문구, 배치, 상태, 해상도 목록은 기존
계약 그대로이며 이 보정은 누락된 증거 생성 경로만 닫는다.

## 정확한 구현 표면

- `Assets/AcadeGameMaker/Editor/HubAuthoring/HubPresentationCaptureGenerator.cs`
- 같은 경로 `.meta`, 고정 GUID `4f0211caf1394d76a959b6df9ab89477`
- result log `artifacts/unity-results/m5d7qa-20260923/resolution-capture.log`
- 기존에 허용된 `resolution-captures/`의 열 PNG와 `manifest.json`

도구는 `AcadeGameMaker.Hub.Authoring.Editor.HubPresentationCaptureGenerator.Capture`
정적 entrypoint만 노출한다. 기존 authoring asmdef 참조로 충분하며 package,
ProjectSettings, runtime assembly 또는 authored scene/prefab을 바꾸지 않는다.

## 렌더 생명주기

1. canonical validator로 scene, prefab, TMP settings, 두 정적 font/material/atlas,
   Mobile SDF shader를 먼저 관찰 검증한다.
2. 현재 Editor scene setup과 active scene을 기록하고 `Hub.unity`를 Single로 연다.
   디스크 scene/prefab bytes와 GUID를 사전 해시한다.
3. 실제 scene의 `HubMenuRoot`만 사용한다. 임시 Camera와 RenderTexture를 만들고,
   root Canvas를 메모리에서 `ScreenSpaceCamera`로 바꿔 target 크기에 렌더한다.
   presenter/input/router/latch lifecycle을 실행하거나 reflection으로 private state를
   주입하지 않는다. 캡처용 정적 view는 내부 authoring seam으로 네 button에 정확한
   Korean copy, enabled Continue, Continue focus, absent notification을 적용한다.
4. safe frame은 runtime 공식과 동일한
   `floor(min(width/640,height/360))`, centered rect를 사용한다. 0이면 runtime과
   동일하게 visual scale 1을 유지하되 `supported=false`, interaction suppressed다.
   도구는 `HubSafeFrameScalerV1.Refresh()`나 `Screen.width/height`를 호출하지 않는다.
   RenderTexture target dimension을 유일한 capture 입력으로 삼아 같은 공식을 독립
   oracle로 계산하고 실제 `SafeFrame` RectTransform에 메모리에서 직접 적용한다.
   실제 runtime scaler/input/resize semantics는 PlayMode XML이 담당하고 PNG는 이
   authoring oracle의 시각 증거만 담당한다.
5. `Canvas.ForceUpdateCanvases`, TMP mesh update, `Camera.Render` 뒤 target을 RGBA32
   `Texture2D`로 읽고 `EncodeToPNG`한다. PNG는 실제 uGUI/TMP geometry와 canonical
   material/shader를 사용한다. placeholder, OS font, screen scraping은 금지한다.
6. 모든 임시 object/texture/camera를 `DestroyImmediate`하고 original scene setup을
   복원한다. Hub scene/prefab/font/settings/shader bytes가 사전 해시와 다르면 실패하고
   이번 invocation이 쓴 temporary capture directory만 제거한다.

### 고정 렌더 상태

- Camera: temporary object, position `(0,0,-10)`, orthographic, size
  `height/2`, near/far `0.1/20`, rect `(0,0,1,1)`, pixelRect exact target,
  `SolidColor`, background RGBA `(0,0,0,1)`, cullingMask Default layer only,
  HDR/MSAA/occlusion/dynamic-resolution off, targetDisplay 0.
- Canvas: actual Hub root, temporary `ScreenSpaceCamera`, the camera above,
  planeDistance 1, pixelPerfect true, additionalShaderChannels None. Every
  captured Graphic must remain on Default layer.
- RenderTextureDescriptor: exact width/height, ARGB32, 24-bit depth,
  `msaaSamples=1`, `volumeDepth=1`, `dimension=Tex2D`, `useMipMap=false`,
  `autoGenerateMips=false`, `enableRandomWrite=false`, `sRGB=true`.
- Texture2D readback: RGBA32, mipChain false, linear false. Set and restore
  `RenderTexture.active` and `GL.sRGBWrite`; require a non-Null graphics device
  and ARGB32 render-texture support. Quality/project/color-space settings are
  observed and never changed.

The generator captures before/after values for scene setup and active scene,
`Selection.objects`/active object, `RenderTexture.active`, `GL.sRGBWrite`, Canvas
render state, SafeFrame transform, and disk hashes. A pre-existing dirty loaded
scene, stale `.tmp`/`.prev` capture directory, or unsupported graphics surface
fails before opening Hub. One `finally` block destroys temporary resources,
restores all captured globals/selection/scene setup, and verifies equality.

## 단언과 manifest

각 capture 전에 출력/지원 여부/integer scale/centered safe rect, full-output inert
backdrop, 640x360 logical frame, 네 192x32 hit rect와 24/12 minima, exact Korean copy,
TMP overflow 부재, canonical static font/material/shader, non-color focus/disabled cues,
below-minimum suppression을 자동 단언한다. Resize frame0은 suppressed=true, frame1은
false이며 둘 다 1280x720 layout이다.

TMP observations are exact per path:

- `SafeFrame/MenuPanel/Continue/Label` = `계속하기`
- `SafeFrame/MenuPanel/NewGame/Label` = `새 게임`
- `SafeFrame/MenuPanel/Settings/Label` = `설정`
- `SafeFrame/MenuPanel/Quit/Label` = `종료`
- `SafeFrame/Notification/Dismiss` = `×`; notification root is inactive and
  `SafeFrame/Notification/Message` remains empty for this authoring capture.

For each component the generator checks exact Regular static font asset,
canonical Mobile SDF material shader, Normal wrapping, configured font size,
no fallback/dynamic addition, `preferredWidth <= rect.width`,
`preferredHeight <= rect.height`, `isTextOverflowing=false`, and after mesh
update one rendered character record per non-space source character. These are
authoring-view assertions, not a claim that runtime handoff or input executed.

`stateDigestSha256` preimage는 UTF-8, LF 없음인 정확한 문자열이다.

```text
M5D7QA-CAPTURE-V1|<file>|<width>x<height>|<supported 0/1>|<scale>|<left>,<bottom>,<width>,<height>|<suppressed 0/1>|CONTINUE_ENABLED_FOCUSED|NOTICE_ABSENT
```

PNG SHA는 최종 bytes의 uppercase SHA-256이다. manifest는 계약의 exact field set,
field order, capture filename order를 지키며 모든 `assertionPassed=true`다. 모든
PNG와 manifest를 temporary sibling directory에 먼저 쓴 뒤 완성된 set만 게시한다.
실패하면 기존 유효 set을 보존하고 temporary files만 제거한다.

Manifest bytes are canonical UTF-8 without BOM, two-space indentation, LF-only,
exact field order, invariant base-10 integers, lowercase JSON booleans, no
optional fields/trailing spaces, and exactly one final LF. Filenames contain no
escapes. The generator writes `resolution-captures.tmp`; after validating the
complete set it moves an existing final directory to
`resolution-captures.prev`, moves tmp to final, then deletes prev. On any move
failure it restores prev exactly. Pre-existing tmp/prev is a fail-closed
condition, not repaired. No partial final set is observable after return.

The fixed generator GUID must be absent repository-wide before its own meta is
created and resolve to exactly the canonical generator path afterward; every
other occurrence is rejected.

## 검증과 실패 경계

- 같은 process 두 번째 생성과 fresh process 생성은 PNG/manifest bytes가 동일하다.
- wrong filename/order/size/scale/rect/suppression, digest/hash drift, extra field/file,
  missing PNG, non-PNG signature, text overflow, missing/error/foreign shader, dynamic or
  fallback font, scene/prefab mutation을 각각 거부한다.
- GPU/driver 차이로 fresh-process PNG가 달라지면 자동 승인하지 않고 stop한다.
- PNG는 입력을 증명하지 않으며 기존 PlayMode resize/hit assertions와 함께만
  `AC-M5D7QA-007/010` 증거가 된다.
- Manifest state token `CONTINUE_ENABLED_FOCUSED|NOTICE_ABSENT` explicitly means
  the fixed authoring capture view. It never asserts runtime controller
  readiness, handoff receipt, cursor advancement, pointer quarantine execution,
  or input delivery; those claims remain exclusively XML-backed.

## Luna CONDITIONAL findings closure

- P1-001: RT dimensions now feed an independent explicit oracle, never
  `Screen.width/height`; runtime semantics remain XML-owned.
- P1-002: Camera/Canvas/RT/readback/global render settings are exact above.
- P1-003: tmp/final/prev publish and rollback are directory-atomic and closed.
- P1-004: dirty-scene preflight and finally restoration cover scene setup,
  selection, Canvas/SafeFrame and global render state with equality proofs.
- P1-005: repository-wide fixed-GUID absence/identity is a pre/post condition.
- P1-006: manifest serialization is byte-canonical above.
- P2-001: exact TMP paths and observations are enumerated.
- P2-002: the manifest token is explicitly authoring-only; runtime claims stay
  in the passing PlayMode suite.

## 계약 delta

- exact implementation allowlist에 generator/meta를 추가한다.
- persistent result allowlist에 `resolution-capture.log`를 추가한다.
- `REQ-M5D7QA-006/009`, `AC-M5D7QA-007/010`에 real-scene rendering,
  assertions, immutability, repeatability를 연결한다.
- focused/full XML 통과 뒤 capture same/fresh no-op을 실행하고, 다음 scope/final
  manifests를 만든다.
- rollback은 generator/meta, capture log, 이번 단위 capture set만 제거한다.

Luna가 P0/P1을 독립 검토하고 Astra가 `Approved`를 복원하기 전에는 구현하지 않는다.
