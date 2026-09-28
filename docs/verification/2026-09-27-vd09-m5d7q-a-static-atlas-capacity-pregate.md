# VD-09 M5D7Q-A static-atlas capacity amendment — Luna 독립 사전검토

- 검토일: 2026-09-27
- 검토자: Luna (`gpt-5.6-luna`)
- 대상 계약 SHA-256: `A53BE2B0F749D41619FC344E2E1C42B6D036873299216727E04E81D586FCE797`
- 대상 제안 SHA-256: `83E23805F8D6E6C24461B64133EB3EC01D738795AA76DD67B441934617C73420`
- 범위: 2048² capacity amendment의 최소성·결정성, Alpha8/mipmap/raw
  payload 수치, `SystemInfo.maxTextureSize` precondition, Regular/Bold pair
  atomicity/cleanup, 138 glyph coverage, mutation/REQ/AC/result coverage,
  기존 shader/TMP Settings/license/no-dynamic/no-multi 회귀
- 구현 여부: 구현하지 않음. 현행 1024 blocker와 package source 및 계약만
  독립 검토했으며 Unity 자산·테스트·result 파일은 생성하지 않음.

## 확인된 원인과 현재 상태

Terra의 보존된 실제 로그는 승인된 shader/TMP Settings preflight 이후
`pointSize=90`, `padding=9`, `SDFAA`, `1024×1024`, `multi-atlas=false`에서
정확한 138-code-point population을 시도했고, 마지막 세 필수 음절
`하항했`을 missing으로 보고했다.

`하`, `항`, `했`은 `AST-UI-FONT-001.md`의 고정 42개 Hangul 집합에 포함되며,
삭제·대체·fallback·runtime addition으로 해결할 수 없다. 로그는
`artifacts/unity-results/m5d7qa-20260923/builder-pass-b.log`에 보존되고,
실패 invocation이 만든 Regular SDF target은 계약에 따라 제거되었다. 기존
shader, TMP Settings, OTF/OFL, license 및 무관한 경로는 변경되지 않았다.

## Alpha8·mipmap·material 수치의 독립 검산

설치된 uGUI 2.6.0 `TMP_FontAsset.cs`를 읽어 다음을 확인했다.

- non-color `SDFAA` branch는 `TextureFormat.Alpha8`을 선택한다.
- atlas texture는 `new Texture2D(1, 1, texFormat, false)`로 만들어지고,
  실제 atlas dimension으로 `Resize(width, height, texFormat, false)`된다.
  `false`는 mip chain을 만들지 않는 경계다.
- SDF material은 `_TextureWidth`, `_TextureHeight`를 atlas dimension으로
  설정하고 `_GradientScale`은 `atlasPadding + packingModifier`로 설정한다.
  SDFAA의 packing modifier가 1이므로 padding 9에서 gradient scale 10은
  계약과 일치한다.

Unity 6 API도 `SystemInfo.maxTextureSize`를 현재 graphics hardware가
지원하는 최대 texture width/height로 정의한다. 따라서
`SystemInfo.maxTextureSize >= 2048`를 SDF 생성 전 fail-closed precondition으로
두는 것은 타당하다. 다만 이 값은 충분한 메모리까지 보장하지 않으므로,
allocation/resize 실패는 pair cleanup 경로로 처리되어야 하며 성공으로
추정해서는 안 된다.

Alpha8 raw level-zero 산술은 다음과 같다.

| profile | face당 payload | 두 face | 기존 1024² pair 대비 |
|---|---:|---:|---:|
| 1024×1024 | 1,048,576 bytes | 2,097,152 bytes | — |
| 2048×2048 | 4,194,304 bytes | 8,388,608 bytes | +6,291,456 bytes |

PowerShell 독립 계산도 `2048*2048=4,194,304`, 두 face `8,388,608`, 기존
pair `2,097,152`, delta `6,291,456`을 확인했다. 이는 Alpha8 raw payload만
뜻하며 serialized YAML, object metadata, readable CPU copy, driver allocation,
build compression을 포함하지 않는다. 제안서가 이 범위를 분리해 기록한 점은
정확하다. `mipmapCount == 1` 및 raw-data length는 구현 후 실제 texture에서
증명해야 한다.

현재 builder의 Glyphs literal을 C# escape 해석 후 독립 순회한 결과는
`count=138`, `unique=138`, code-point 오름차순이다. 따라서 proposal의
exact-set/duplicate/missing 검사 요구는 현행 source 집합과 일치한다.

## 2048² 최소성 검토 — P1 발견

2048²는 승인된 point size 90, padding 9, SDFAA, single-atlas, Static,
no-fallback, no-runtime-addition을 그대로 보존하는 보수적 변경이다. 1024²가
실패했으므로 다음 **power-of-two 정방형** 후보인 2048²를 채택하는 논리는
실용적이고, 4배 면적은 3 glyph만 넘친 실패에 충분한 여유를 제공할 가능성이
높다. 그러나 현재 제안서와 계약은 아틀라스 dimension을 square/power-of-two로
제한한다는 규범을 명시하지 않는다.

uGUI 2.6.0의 public path는 `atlasWidth`, `atlasHeight`를 받아 free rectangle을
그대로 만들고 texture를 해당 크기로 resize하며, package source에서 비POT 또는
비정방형을 거부하는 guard를 찾을 수 없다. Unity `Texture2D`/texture import
surface도 일반적으로 NPOT를 별도 지원 조건으로 다룬다. 따라서 품질 변수
(font face, point, padding, render mode, glyph set, static/single/no-multi)를
그대로 유지한 채 `1536×1536` 또는 `1024×1536` 같은 더 작은 dimension-only
후보를 기술적으로 배제할 근거가 현재 문서에 없다.

이것은 2048²가 잘못됐다는 뜻이 아니다. 다만 “가장 작은
quality-preserving deterministic change”라는 강한 주장은 아직 증명되지
않았다. 다음 둘 중 하나가 필요하다.

1. 이 단위의 dimension policy를 **square power-of-two만 허용**으로 명시하고,
   그 이유(Windows baseline의 cross-platform texture determinism 및 packer
   profile 고정)를 REQ/AC와 mutation matrix에 추가한다. 그러면 실패한
   1024² 다음 admissible profile인 2048²가 최소가 된다.
2. 또는 source-controlled 자산을 쓰지 않는 bounded capacity sweep으로
   1024×장변/1536² 후보를 실제 package version에서 비교해, 품질 변수를
   바꾸지 않고 성공하는 가장 작은 dimension을 선택한다. 이 경우 proposal의
   고정 2048 mutation policy와 raw-memory 수치는 다시 갱신해야 한다.

현재 문서에는 1 또는 2가 반영되어 있지 않으므로 `P1-ATLAS-001`을 열었다.
이는 사용자 제품 결정을 요구하지 않는 기술 계약 보정이다.

## Pair atomicity·cleanup 및 회귀 범위

제안/계약의 lifecycle 경계는 충분히 구체적이다.

- `SystemInfo.maxTextureSize >= 2048`와 모든 source/settings/shader/license
  preflight 뒤에만 후보를 만든다.
- Regular/Bold를 각각 한 번 생성하고 exact 138 glyph를 한 번 population하며,
  missing/texture count/dimension/format/mipmap/atlasIndex/coverage가 하나라도
  틀리면 adaptive retry 없이 중단한다.
- 두 face의 in-memory structural validation이 모두 끝난 뒤에만 Static으로
  고정하고 canonical target을 저장한다.
- 한 face 실패 시 새 durable pair를 남기지 않으며, cleanup은 해당 invocation의
  unsaved object와 새로 만든 SDF target에만 한정한다. 기존 valid face와
  OTF/OFL, TMP Settings, shader/include/license/notice는 건드리지 않는다.
- width/height, point/padding, render mode, format, mipmap, texture count,
  multi-atlas, atlasIndex, missing/extra/duplicate glyph, material texture/
  gradient/foreign source/shader/fallback 및 either-face partial pair를 AC와
  focused tests에 매핑했다.

이 lifecycle은 구현 계약으로는 PASS다. 다만 `AssetDatabase.CreateAsset` 또는
`SaveAssets`가 첫 face 저장 후 실패하는 persistence exception에서도 partial
pair가 남지 않는지, Terra 구현과 독립 mutation test가 실제로 증명해야 한다.
이는 post-implementation evidence 항목이며 현재 계약상 P0/P1을 추가하지
않는다.

기존 Mobile SDF shader의 exact bytes/name/include/GUID, TMP Settings의
`modern-Hangul=true`·non-null empty feature list·null line-breaking/default/
fallback 정책, Companion/OFL license notice, Static/no-fallback/no-runtime
addition/no-multi 경계는 2048 amendment에서 변경되지 않는다. UI logical size,
Korean copy, layout/capture 기준도 유지된다.

## Traceability·result coverage

- `REQ-M5D7QA-007/009`가 2048 single-atlas profile과 pair-atomic lifecycle을
  직접 규정한다.
- `AC-M5D7QA-002`가 dimension/format/mipmap/count/multi/glyph/material 및
  partial-pair mutation을 거부한다.
- `AC-M5D7QA-007`이 atlas dimension 변경으로 logical layout/copy/readability가
  바뀌지 않음을 확인한다.
- `AC-M5D7QA-008`이 max texture size, Alpha8/no-mip/raw payload, exact glyph
  set, material values, source/shader identities와 generated hashes를 확인한다.
- `AC-M5D7QA-010`이 1024 negative (`하항했`), Regular/Bold 2048 positive,
  same/fresh no-op, full regression and zero fail/skip/inconclusive를 요구한다.
- result allowlist의 `builder-pass-c.log`, `builder-fresh-process-2048.log`,
  focused EditMode/XML/log 및 기존 blocker-log 보존 규칙은 명시되어 있다.

문서 동기화 참고(초기 검토 시점의 snapshot): 당시 `AST-UI-FONT-001.md`의
normative table은 1024×1024를 기록했고, `docs/README.md`와
`docs/specs/README.md`의 M5D7Q-A 상태 표기도 기존 Approved를 보일 수 있었다.
제안/계약은 Astra 승인 후 normative dimension/raw-payload만 갱신하고
historical 1024 miss를 보존하도록 명시했다. 이는 구현 전 문서 동기화가
필요한 `P2-ATLAS-001`이었다. 현재 상태는 아래 Closure re-gate에서 재검증한다.

## 판정

| 항목 | 판정 |
|---|---|
| 실제 blocker와 2048 response 연결 | PASS |
| Alpha8/mipmap/raw memory 산술 | PASS |
| maxTextureSize gate | PASS, memory allocation은 후속 fail-closed 검증 필요 |
| pair-atomic lifecycle/cleanup 계약 | PASS, 구현·저장 exception은 post-gate 필수 |
| exact 138 glyph set / missing triad | PASS |
| mutation/REQ/AC/test/result coverage | PASS |
| shader/TMP Settings/license/no-dynamic/no-multi 회귀 | PASS |
| “2048²가 가장 작은 변경” 주장 | **P1-ATLAS-001 OPEN** |
| AST/font evidence 및 published index 동기화 | **P2-ATLAS-001 OPEN** |

**초기 검토 판정(문서 보정 전 snapshot): CONDITIONAL — P0=0, P1=1, P2=1.**

Astra는 새 사용자 제품 결정을 받을 필요는 없지만, 초기 검토 시점에는
`Approved` 복원이 금지되었다. P1/P2 closure 결과와 현재 승인 준비 상태는
아래 Closure re-gate가 우선한다. 구현 뒤에는 여전히 Luna post-review에서
실제 Regular/Bold texture 구조·missing set·pair cleanup·same/fresh no-op 및
전체 회귀를 독립 검증해야 한다.

권위 문서:

- [capacity amendment proposal](../proposals/2026-09-27-vd09-m5d7q-a-static-atlas-capacity-amendment.md)
- [M5D7Q-A contract](../specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md)
- [Terra blocker evidence](2026-09-23-vd09-m5d7q-a-implementation-evidence.md)
- [Noto font evidence](../assets/evidence/AST-UI-FONT-001.md)
- [Unity `SystemInfo.maxTextureSize` documentation](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/SystemInfo-maxTextureSize.html)

## Closure re-gate — 2026-09-27

이번 재검토는 위의 초기 CONDITIONAL 판정에서 열린 두 문서 finding만 좁게
재검증했다. 구현이나 Unity 자산 생성은 하지 않았다.

### 재검증 대상과 현재 SHA-256

| 대상 | SHA-256 |
|---|---|
| capacity amendment proposal | `B0BBFE2D0F0A683A5DF7C7FD0DEE97D065383A1DF0E3331F0A709F8078C5CD62` |
| M5D7Q-A Review contract | `19878DBC933BABA0D816E71154C6D8142D0F865101B9E20D593A96727C1BEC40` |
| `AST-UI-FONT-001.md` | `0BE7D9EA2D130E8567A7F4B1B9C3FB2D347E65F957344EFECB5F663D2CB8B976` |
| `docs/README.md` | `C4577B357ECFDC8C9E2BE33EA04323FF2FB0D9651DBB1C81A0D000538AB9DB6F` |
| `docs/specs/README.md` | `B8BB7379A50BB0B2946ECE82727961AFDC1242BB59411F1873C61D5A6E85CFE0` |

### P1-ATLAS-001 closure

계약과 proposal 모두 project-owned static TMP atlas를 **모든 face에서 동일한
정사각형(square) 2의 거듭제곱(power-of-two) dimension profile 하나만** 허용하는
정책으로 명시했다. 이 정책은 결정적인 플랫폼/직렬화/압축 및 packing profile을
고정하기 위한 프로젝트 규칙이며, uGUI나 Unity가 NPOT 또는 직사각형 texture를
기술적으로 거부한다는 주장이 아니다. 따라서 `1024x1024` 실패 뒤의 다음이자
가장 작은 **허용 profile**이 `2048x2048`이라는 논리가 정확해졌다. 임의의
직사각형 중 수학적 최소라는 주장은 하지 않는다.

계약의 mutation matrix도 이 정책과 일치한다. `1024x1024`, `4096x4096`,
`1024x2048`, `1536x1536`, 기타 non-square/non-POT 또는 exact `2048x2048`
이외 dimension을 각각 거부하며, point/padding/render mode/format/mipmap/
texture count/multi-atlas/atlas index/glyph/material/source/shader/fallback/
partial-pair 변이도 독립적으로 거부하도록 REQ/AC/test에 연결되어 있다.

### P2-ATLAS-001 closure

`AST-UI-FONT-001.md`의 현재 normative table은 두 face 모두 `2048x2048`로
동기화되었고, 이전 `1024x1024`에서 `하항했`이 누락된 실제 blocker는
historical evidence로 보존된다. `docs/README.md`와 `docs/specs/README.md`의
M5D7Q-A index는 `Review (proposed 2048x2048 atlas)`와 Luna 재검토/Astra 승인
대기 상태를 가리키며 stale `Approved` 또는 현재 normative `1024x1024` 표기를
남기지 않는다.

### 최종 closure 판정

| 항목 | 결과 |
|---|---|
| P0 | 0 |
| P1-ATLAS-001 | **CLOSED** |
| P2-ATLAS-001 | **CLOSED** |
| 현재 계약/문서 pre-gate | **PASS — `Approved` 복원 가능** |

새 사용자 제품 결정은 필요 없다. Astra는 위의 최신 proposal/contract SHA를
고정하고 `Review → Approved`를 복원할 수 있다. 이는 구현 성공을 선취하는
승인이 아니다. Approved 복원 뒤에도 Terra는 exact 2048 Regular/Bold positive
fixture, Alpha8/no-mip 구조, pair-atomic cleanup, same/fresh no-op 및 전체
회귀 증거를 제출해야 하며, Luna가 이를 독립 post-review로 확인하기 전에는
완료/통합으로 판정할 수 없다.
