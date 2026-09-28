# M5D7Q-A 정적 TMP 아틀라스 용량 보정 제안

- 날짜: 2026-09-27
- 설계·반례 검토: Sol (`gpt-5.6-sol`)
- 결정 권한: Astra
- 상태: 계약 보정 제안 — 구현 권한 아님
- 대상 계약:
  `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- 기준 계약 SHA-256:
  `692A15E859A1B81068392C5C30A70A70EF272DDF37B90DD1F1632DBB402326F5`
- Luna capacity pre-gate 대상 계약 SHA-256:
  `A53BE2B0F749D41619FC344E2E1C42B6D036873299216727E04E81D586FCE797`
- 2026-09-27 보정: `P1-ATLAS-001`과 `P2-ATLAS-001`의 문서 보정을
  반영했다. 독립 Luna 재검토와 Astra 승인은 아직 대기 중이다.
- 원인 증거:
  `docs/verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md`
  의 `Resume evidence — fixed-atlas glyph-capacity stop`과
  `artifacts/unity-results/m5d7qa-20260923/builder-pass-b.log`

## 결론

Regular와 Bold를 모두 다음 하나의 고정 profile로 바꾼다.

| setting | amended exact value |
|---|---|
| sampling point size | `90` 유지 |
| atlas padding | `9` 유지 |
| glyph render mode | `SDFAA` 유지 |
| atlas width / height | **`2048 / 2048`** |
| atlas count per face | `1` 유지 |
| multi-atlas support | `false` 유지 |
| population | exact sorted 138 code points 유지 |
| final population mode | `Static` 유지 |
| fallback/runtime glyph addition | 금지 유지 |

즉, 유일한 제품 자산 설정 변화는 face당 atlas 변을 1024에서 2048로
확대하는 것이다. copy, 글꼴 face, 글리프 집합, point size, padding, SDF
gradient, UI text size와 layout은 바꾸지 않는다.

이 선택에서 말하는 “최소”는 임의의 직사각형 texture 중 수학적으로 가장 작은
면적이라는 뜻이 아니다. 이 프로젝트의 source-controlled 정적 TMP atlas는
모든 face에 동일한 **정사각형 2의 거듭제곱(square power-of-two)** width/height만
허용한다. 이미 선택된 1024x1024 profile이 실제 package run에서 실패했으므로,
그보다 큰 허용 profile 중 다음이자 가장 작은 값이 2048x2048이다. uGUI public
API가 1536x1536이나 1024x1536을 기술적으로 받을 수 있다는 사실은 부정하지
않는다. 그런 임의 직사각형/NPOT profile을 본 프로젝트의 결정적 자산 문법에서
의도적으로 제외한다.

새 사용자 제품 결정은 필요 없다. Windows x64 수직 데모에서 두 Alpha8 atlas의
raw payload가 합계 6 MiB 증가하는 대신, 승인된 품질·copy·단일 정적 atlas
문법을 그대로 지키는 기술 보정이다. Luna의 좁은 사전검토와 Astra의 재승인
전에는 구현을 재개할 수 없다.

## 실제 실패의 의미

승인된 shader와 settings preflight를 통과한 실제 Unity 6000.6.0f1 run은
Regular face를 `pointSize=90`, `padding=9`, `SDFAA`, `1024×1024`, multi-atlas
false로 생성한 뒤 exact sorted 138-character string을 한 번 전달했다.
uGUI 2.6.0의 public `TryAddCharacters`는
`FontEngine.TryAddGlyphsToTexture(..., GlyphPackingMode.BestShortSideFit, ...)`
를 사용하고, atlas가 가득 차면 multi-atlas가 false이므로 남은 glyph를 새
texture에 넣지 않는다. 결과는 exact final three Hangul syllables
`하항했` 누락이었다.

이 세 음절은 실제 승인 copy에 필요하며 font에도 존재한다. 앞선 135 glyph가
같은 호출에서 들어갔고 마지막 세 glyph만 pack되지 않았으므로 source-font
누락이나 문자열 오류가 아니라 고정 atlas 수용량 실패다. 글리프 제거·대체,
fallback 또는 runtime addition은 해결책이 아니다.

## 대안 비교

### A. 2048×2048, point 90, padding 9 — 채택

- 같은 font face와 같은 point/padding으로 같은 glyph bitmap과 SDF 여백을
  생성한다. 달라지는 것은 atlas 배치 좌표와 texture dimension뿐이다.
- 1024×1024의 4배 면적으로 138 glyph에 충분한 여유를 주며 packer의 작은
  fragmentation 차이에 의존하지 않는다.
- Regular/Bold가 같은 profile을 유지해 validator, 재생성, 시각 비교가 단순하다.
- 단일 atlas, static, no fallback, no runtime addition 조건을 모두 유지한다.
- 정사각형 POT 고정은 width/height 방향에 따른 별도 packing profile을 없애고,
  Unity의 플랫폼별 texture import/serialization/compression 경계를 하나의
  검증 가능한 profile로 제한한다. 이는 NPOT가 Unity/uGUI에서 지원되지 않는다는
  주장이 아니라, cross-platform 결정성과 source-control 감사를 위한 프로젝트
  정책이다.
- 따라서 2048x2048은 arbitrary rectangle의 수학적 최소라고 주장하지 않는다.
  실패한 1024x1024보다 큰 square-POT 후보 중 **다음이며 가장 작은 허용
  profile**이다. 실제 138-glyph 수용 성공은 구현 후 Regular/Bold positive
  fixture로 별도 증명해야 한다.
- Windows x64 baseline에서 2048 texture는 보수적인 크기다. Builder와 Windows
  검증은 `SystemInfo.maxTextureSize >= 2048`을 fail-closed precondition으로
  확인한다. Unity의 공식 API 정의는 이 값이 현재 graphics hardware가 지원하는
  최대 width/height라고 명시한다:
  <https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SystemInfo-maxTextureSize.html>

### B. 1024×1024에서 padding 축소 — 반려

- 예를 들어 9→8은 packing 면적을 줄일 가능성이 있지만 실제 Regular/Bold
  둘 다 exact 138 glyph가 들어간다는 증거가 없다.
- Mobile SDF material의 `_GradientScale`은 package source상 `padding + 1`이다.
  padding 변경은 10→9로 SDF edge/outline 여유를 바꾸므로 단순 용량 보정이
  아니라 렌더 품질 변경이다.
- 어느 최소값에서 pack되는지를 탐색해 채택하면 packer 결과에 바짝 맞춘
  profile이 되어 engine/package drift와 향후 검증에 취약하다.

### C. sampling point size 축소 — 반려

- 90→89/88 등은 glyph bitmap 면적을 줄이지만 face metrics와 SDF source
  resolution을 함께 바꾼다. 12/14 logical-pixel 한국어의 작은 획 품질을 다시
  시각 승인해야 한다.
- 정확히 어느 값이 두 face 모두에서 성공하는지는 별도 sweep 전에는 알 수
  없으며, packing fragmentation 때문에 “세 glyph만 부족하니 2% 축소” 같은
  산술 추정은 acceptance evidence가 아니다.
- 메모리는 절약하지만 현 단위의 우선순위인 copy 완전성·가독성·재현성을 위해
  atlas 확대보다 열등하다.

### D. face별 point/padding/glyph profile 분리 — 반려

- Bold가 현재 강조에만 예약되어 있어도 계약은 두 face가 같은 138 glyph를
  제공하도록 정했다. Bold subset이나 더 작은 point를 쓰면 향후 강조 copy에
  다른 coverage/quality를 만들고 face별 mutation matrix를 늘린다.
- Regular 2048/Bold 1024 같은 비대칭도 Bold의 실제 성공 증거가 없으며, 같은
  원본·동일 profile이라는 기존 감사 이점을 없앤다.

### E. 두 atlas, fallback 또는 glyph split — 금지 유지

- multi-atlas=true, face를 여러 TMP asset으로 분리, fallback 연결은 모두
  runtime selection surface를 늘린다. 본 단위의 one-atlas-per-face/static/no-
  fallback 계약과 충돌하므로 고려하지 않는다.

## 메모리·저장 비용

uGUI 2.6.0 `TMP_FontAsset.CreateFontAssetInstance`는 non-color `SDFAA` atlas를
`TextureFormat.Alpha8`로 만들며 `new Texture2D(..., false)`를 사용해 mipmap을
만들지 않는다. 따라서 image payload의 결정적 산술값은 다음과 같다.

| profile | per face raw Alpha8 payload | two faces | delta from old target |
|---|---:|---:|---:|
| 1024×1024 | 1,048,576 bytes | 2,097,152 bytes (2 MiB) | — |
| 2048×2048 | 4,194,304 bytes | 8,388,608 bytes (8 MiB) | +6,291,456 bytes (6 MiB) |

이는 atlas image payload이며 Unity object metadata, material/font tables,
Editor의 readable CPU copy, serialized YAML/hex overhead, player build
compression과 driver allocation까지 포함한 총 resident/build 크기라고 주장하지
않는다. 구현 증거는 다음 실제값을 별도로 기록한다.

- 각 embedded atlas의 `format`, width/height, `mipmapCount`, raw-data length;
- 각 SDF `.asset`의 filesystem byte size와 SHA-256;
- `Profiler.GetRuntimeMemorySizeLong` 또는 동등한 Unity-reported per-texture
  값이 안정적으로 사용 가능하면 그 값과 측정 환경;
- final manifest의 두 SDF asset 및 embedded texture/material identity.

프로젝트는 출시 대상이 아닌 Windows PC 데모이고, 승인된 copy/가독성/정적
결정성을 보존하는 대가로 6 MiB raw 증가를 수용하는 것이 합리적이다. 실제
Windows build size는 compression과 serialization에 좌우되므로 사전 숫자를
꾸미지 않고 후속 build gate가 있을 때 측정한다.

## Builder 생명주기 경계

계약 승인 뒤 Terra가 바꿀 수 있는 생성 상수는 atlas width와 height의
`1024 → 2048`뿐이다. point 90, padding 9, SDFAA, exact Glyphs string,
multi-atlas false와 existing source/shader/settings 경계는 그대로다.

1. SDF 생성 전에 exact font/settings/shader/license preflight와
   `SystemInfo.maxTextureSize >= 2048`을 모두 확인한다.
2. Regular와 Bold 각각 public `CreateFontAsset`을 exact amended profile로 한 번
   호출하고 exact sorted 138 string을 한 번 population한다.
3. 하나라도 `TryAddCharacters=false`, missing non-empty, texture count != 1,
   wrong dimension/format/mipmap, atlasIndex != 0이면 자동 축소·재시도·fallback·
   second atlas 없이 중단한다.
4. 두 face의 in-memory structural validation이 모두 끝난 뒤에만 canonical SDF
   target을 저장한다. 한 face 성공 후 다른 face 실패로 partial durable asset을
   남기지 않는다.
5. 저장 전 `Static`으로 동결하고 source runtime reference, fallback, runtime
   feature/addition과 multi-atlas 권한을 제거한 기존 계약을 유지한다.
6. valid canonical assets가 이미 있으면 builder는 byte-for-byte no-op이다.

실패 cleanup은 그 invocation의 unsaved in-memory objects와 새로 생성 중인 두
SDF target에만 한정한다. source OTF/OFL, TMP Settings, shader/include/license,
기존 prefab/scene 또는 다른 자산을 바꾸지 않는다.

## Validator와 독립 반례

최종 Regular/Bold 각각에 대해 다음을 observational하게 증명한다.

- `atlasPopulationMode=Static`, `atlasWidth=2048`, `atlasHeight=2048`,
  `atlasPadding=9`, `atlasRenderMode=SDFAA`;
- `atlasTextures.Length=1`, texture `2048×2048`, `TextureFormat.Alpha8`,
  `mipmapCount=1`, raw payload `4,194,304` bytes;
- multi-atlas false, fallback list null/empty, runtime glyph addition 불가;
- character table은 exact sorted 138 code points와 set-equal이고 누락·추가·중복이
  없으며 모든 glyph의 `atlasIndex=0`이고 rect가 atlas bounds 안이다;
- face info point size 90, source OTF GUID/hash가 정확하다;
- material `_MainTex`가 sole atlas이고 `_TextureWidth=2048`,
  `_TextureHeight=2048`, `_GradientScale=10`, canonical Mobile SDF shader를
  직접 참조한다;
- 두 SDF/atlas/material의 local identity, final bytes/hash, same-process 및
  fresh-process no-op bytes가 고정된다.

독립 mutation tests는 최소한 width/height 1024, 4096, 비정사각형
`1024x2048`, NPOT 정사각형 `1536x1536`, 다른 NPOT/비정사각형 또는 exact
2048x2048이 아닌 dimension, point 89, padding 8, non-SDFAA, RGBA32,
mipmap >1, texture count 0/2,
multi-atlas true, atlasIndex 1, missing `하`/`항`/`했`, extra glyph, material
dimension/gradient/texture drift, foreign shader/fallback/source를 각각 거부한다.

추가로 in-memory negative capacity fixture는 exact Regular/90/9/SDFAA/
1024×1024/no-multi profile이 observed blocker와 동일하게
`missing == "하항했"`임을 고정 package/version에서 재현하고, amended
2048×2048 profile은 Regular와 Bold 모두 missing empty임을 증명한다. 이 fixture는
project asset을 쓰지 않고 생성 objects를 즉시 정리한다.

PlayMode와 named captures는 640×360에서 12/14 logical-pixel 메뉴·최장 알림의
모든 승인 한글이 missing-glyph substitution, truncation, overlap, blur 없이
보이고, 1280×720·1920×1080·2560×1440 정수 확대에서도 동일 layout/readability를
유지하는지 확인한다.

## 정확한 계약 delta와 상태 게이트

최신 계약은 이 blocker가 발견된 즉시 `Approved → Review`로 내려야 한다.
원 승인, TMP Settings 승인, shader 승인 이력은 모두 보존하되 atlas-capacity
보정은 새 Luna/Astra gate로 구분한다.

다음 문구만 normative하게 바꾼다.

- Approval gate의 “1024×1024 실패 시 stop” 이력 뒤에 실제 `하항했` capacity
  stop과 2048 amendment pending 상태를 기록한다.
- Korean/static-font section과 `AST-UI-FONT-001.md`의 두 face 설정을
  `90 / 9 / SDFAA / 2048×2048 / single / no multi`로 바꾼다.
- `REQ-M5D7QA-007`은 exact 2048 single-atlas static profile과 complete 138
  coverage, 그리고 square-POT-only 정적 atlas 정책을 명시한다.
- `REQ-M5D7QA-009`은 두 face를 저장 전에 함께 검증하고 partial durable SDF를
  남기지 않는 lifecycle을 명시한다.
- `AC-M5D7QA-002`에 point/padding/dimension/format/mipmap/atlas-count/
  multi-atlas/glyph-set/material mutation 및 failed-pair atomicity를 추가하고,
  `1536x1536`, `1024x2048`, 다른 non-POT/non-square/비2048 dimension을
  독립적으로 거부한다.
- `AC-M5D7QA-007`은 2048 atlas가 UI logical size를 바꾸지 않고 기존 named
  captures에서 copy/readability를 유지함을 확인한다.
- `AC-M5D7QA-008`은 위 exact structural values, raw payload 산술값,
  source/material/shader identities와 final hashes를 추가한다.
- `AC-M5D7QA-010`은 1024 negative capacity fixture, Regular/Bold 2048 positive
  fixture, same/fresh no-op 및 기존 전체 suite를 요구한다.
- allowlist 파일 경로는 늘리지 않는다. builder/validator/tests, 이 proposal,
  contract, `AST-UI-FONT-001`, Terra/Luna evidence와 index hunk 안에서 끝낸다.
- 기존 blocker logs는 덮어쓰지 않는다. result allowlist에 정확히
  `builder-pass-c.log`와 `builder-fresh-process-2048.log`만 추가한다.
  same-process second invocation과 mutation 결과는 `focused-editmode.xml/.log`
  에 남긴다.
- rollback은 amended SDF assets만 제거하고 font/settings/shader prerequisites와
  과거 blocker evidence를 보존한다.

Luna가 이 proposal과 amended contract를 package source/실제 blocker에 대조해
`P0=0`, `P1=0`을 보고한 뒤 Astra만 `Approved`를 복원할 수 있다. Terra는 그
전까지 constants, font evidence, tests, Unity assets 또는 result files를 바꾸지
않는다. 승인 뒤 첫 2048 run이 실패하면 자동으로 point/padding을 낮추지 않고
다시 stop한다.

## Luna capacity pre-gate 보정 응답

- `P1-ATLAS-001`: square-POT-only project policy를 proposal, 계약
  `REQ-M5D7QA-007`, `AC-M5D7QA-002/008`, font evidence에 명시했다. 2048x2048은
  arbitrary rectangle의 수학적 최소가 아니라 실패한 1024x1024보다 큰 profile
  중 다음이자 가장 작은 **허용** profile이다. Validator는 exact 2048x2048
  외에 `1024x1024`, `4096x4096`, `1024x2048`, `1536x1536`과 다른
  non-square/non-POT/other dimensions를 독립 거부한다.
- `P2-ATLAS-001`: `AST-UI-FONT-001.md`를 proposed/Review 2048 profile로
  동기화하고 1024 `하항했` 실패를 historical evidence로 보존했다.
  `docs/README.md`와 `docs/specs/README.md`도 stale Approved 표기를 Review로
  바꿨다.

이 응답은 두 finding에 필요한 문서 보정을 제출한 것이며 Luna의 독립 closure를
대신하지 않는다. 계약은 `Review`이고 Astra 승인 전 구현 권한은 없다.

## 사용자 결정

필요 없다. 사용자가 승인한 한국어 copy, UI logical sizes, font family/weight,
정적 단일-atlas/no-fallback 품질 방향은 그대로다. 선택지는 기술적으로 다음과
같이 닫힌다.

- 6 MiB raw atlas payload 증가를 감수해 시각·copy 계약을 그대로 유지하거나;
- 더 작은 memory를 위해 padding/point/face coverage를 바꾸고 별도의 시각 제품
  결정을 다시 여는 것.

현재 Windows PC 비출시 데모의 규모에서는 전자가 명백히 낮은 위험이므로
Astra 기술 승인 범위다.
