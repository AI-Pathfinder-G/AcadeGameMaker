# M5D7Q-A 최소 TMP Mobile SDF 셰이더 보정 제안

- 날짜: 2026-09-27
- 설계·반례 검토: Sol (`gpt-5.6-sol`)
- 결정 권한: Astra
- 상태: 계약 보정 제안 — 구현 권한 아님
- 대상 계약:
  `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- 기준 계약 SHA-256:
  `DB4315900D5BC02955BB2979257713B369788CE687A8ED66C4A5E5DC7DEA2019`
- 원인 증거:
  `docs/verification/2026-09-23-vd09-m5d7q-a-implementation-evidence.md`
  및 `artifacts/unity-results/m5d7qa-20260923/builder-pass-b.log`,
  `builder-fresh-process.log`

## 결론

전체 `TMP Essential Resources.unitypackage`를 임포트하지 않는다. 설치되어
잠긴 uGUI 2.6.0 패키지의 해당 묶음에서 **정확히 두 원본 파일**만 바이트 그대로
프로젝트 소유 경로에 복제한다.

1. `TMP_SDF-Mobile.shader`
2. 그 셰이더가 직접 include하는 `TMPro_Properties.cginc`

이 두 파일과 고정 `.meta`, 그리고 uGUI 패키지의 정확한 저작권·라이선스
고지 사본만 source control에 둔다. 기본 폰트, fallback, style sheet,
line-breaking asset, 다른 TMP 셰이더, package importer, 동적 atlas 권한은
추가하지 않는다. 이 보정은 정적 SDF 생성에 필요한
`Shader.Find("TextMeshPro/Mobile/Distance Field")` 결과만 공급한다.

새 사용자 제품 결정은 필요 없다. 한국어 copy, Noto 글꼴, UI 디자인, 정적
atlas 사양은 변하지 않는 기술 선행조건 보정이다. Luna의 좁은 사전검토와
Astra의 계약 재승인 전에는 구현을 재개할 수 없다.

## 확인된 실패와 공개 API 경계

uGUI 2.6.0의 모든 공개 SDF 생성 overload는 최종적으로 같은 private
`TMP_FontAsset.CreateFontAssetInstance`에 진입한다. 그 메서드는 SDF 재질을
다음과 같이 만든다.

```text
new Material(ShaderUtilities.ShaderRef_MobileSDF)
Shader.Find("TextMeshPro/Mobile/Distance Field")
```

현재 프로젝트에는 그 이름의 셰이더가 없으므로 `Material` 생성자가 null
shader를 받아 `ArgumentNullException`으로 중단된다. `-nographics`를 제거한
fresh process에서도 동일하므로 그래픽 장치 모드의 우연한 실패가 아니다.

### 기존 패키지 참조/API만으로 가능한 우회 검토

| 후보 | 판정 | 이유 |
|---|---|---|
| `CreateFontAsset(Font, ...)`의 다른 overload | 불가 | 모든 SDF overload가 같은 `CreateFontAssetInstance`와 Mobile SDF shader lookup을 사용한다. |
| 파일 경로/패밀리명 overload | 불가 | 같은 내부 경로를 사용하며 패밀리명 경로는 OS font 및 `DynamicOS`까지 도입한다. |
| `TMP_FontAsset_CreationMenu` | 불가 | internal Editor 코드이고 `TextMeshPro/Distance Field`라는 또 다른 Essential Resources shader를 요구한다. |
| 기존 `UI/Default` 또는 임의 URP shader | 불가 | null lookup 이전에 대체 재질을 주입할 공개 seam이 없고 TMP SDF 속성/렌더링 계약도 만족하지 않는다. |
| package의 editor-only internal SDF shader | 불가 | 이름과 목적이 다르고 runtime TMP UI용 shader가 아니다. |
| reflection으로 `ShaderRef_MobileSDF` 주입 | 금지 | runtime reflection/private static mutation 금지와 재현성 경계를 위반한다. |
| FontEngine으로 TMP asset 전체 수동 조립 | 반려 | uGUI 내부 생성 로직을 복제해야 하고 public invariant를 누락하기 쉬우며 현재 bounded unit보다 훨씬 넓다. |
| 임시 import 후 shader 삭제 | 불가 | 최종 SDF material의 shader 참조가 사라져 runtime asset이 깨진다. |

따라서 기존 package reference나 공개 API 선택만으로는 계약을 바꾸지 않고
해결할 수 없다. 영구적이고 직접 참조 가능한 Mobile SDF shader 한 벌이
필수다.

## 정확한 출처와 해시

설치 기준은 `Packages/manifest.json`과 `Packages/packages-lock.json`의
`com.unity.ugui` **2.6.0**, package fingerprint
`23caec89ae2780ae2aa9b14f95a19c03e3dcdf9e`다.

설치 패키지의
`Package Resources/TMP Essential Resources.unitypackage`는 804,874 bytes,
SHA-256
`26CDEE2072683CB25CEAFA4FAB23C93C35B2F692E5DAF9CA0F33D05C4E163274`다.
아카이브 전체를 임포트하거나 저장소에 복사하지 않는다. 아래 두 member의
`asset` byte만 추출한다.

| upstream member | upstream GUID | bytes | exact SHA-256 |
|---|---|---:|---|
| `Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader` | `fe393ace9b354375a9cb14cdbbc28be4` | 8,074 | `44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4` |
| `Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc` | `3997e2241185407d80309a82f9148466` | 2,707 | `66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90` |

대상 source asset은 upstream `asset`과 byte-for-byte 동일하므로 대상 해시도
위 값과 정확히 같아야 한다. shader는 정확한 이름
`TextMeshPro/Mobile/Distance Field`를 유지하고 로컬 include는 정확히
`TMPro_Properties.cginc` 하나다. Unity 제공 `UnityCG.cginc`와
`UnityUI.cginc`는 엔진 include로 남고 프로젝트에 복제하지 않는다.

## 라이선스와 고지

uGUI 2.6.0의 `LICENSE.md`는 431 bytes, SHA-256
`3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6`이며
uGUI copyright와 Unity Companion License 적용을 명시한다. 이 정확한 bytes를
`third_party/unity-ugui-2.6.0-LICENSE.md`에 둔다.

Unity Companion License는 유효한 Unity Engine License와 연결된 authoring/
distribution에서 Work의 재생산·배포를 허용하고, Work의 substantial portion과
함께 license 및 copyright notice를 제공하도록 요구한다. 본 Unity 프로젝트에서
두 원본 파일을 그대로 사용하는 것은 그 용도에 해당하며, package 고지의 정확한
저장소 사본과 아래 증거 문서가 notice를 보존한다.

- 공식 라이선스: `https://unity.com/legal/licenses/unity-companion-license`
- package notice source: 설치된 `com.unity.ugui@2.6.0/LICENSE.md`
- 전용 증거: `docs/assets/evidence/AST-UI-SHADER-001.md`
- asset register: `AST-UI-SHADER-001` 한 행

셰이더 코드나 include를 수정하면 upstream byte identity가 깨지므로 이 보정
범위에서는 수정하지 않는다. 향후 수정본이 필요하면 별도 라이선스·파생물
검토와 계약이 필요하다.

## 정확한 대상 자산과 `.meta`

허용할 프로젝트 경로는 다음뿐이다.

- `Assets/UI/Shaders.meta`
- `Assets/UI/Shaders/Hub.meta`
- `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader` and `.meta`
- `Assets/UI/Shaders/Hub/TMPro_Properties.cginc` and `.meta`
- `third_party/unity-ugui-2.6.0-LICENSE.md`
- `docs/assets/evidence/AST-UI-SHADER-001.md`
- `docs/assets/asset-register.md`의 정확한 한 행

모든 새 `.meta`는 UTF-8 no-BOM, LF, 마지막 LF 포함 canonical YAML이다.

| meta | fixed GUID | importer/schema | bytes | expected SHA-256 |
|---|---|---|---:|---|
| `Assets/UI/Shaders.meta` | `070c509b184d2bca114677820f278a46` | `folderAsset: yes`, empty `DefaultImporter` fields | 172 | `142F6FE56DA2CF181D4E55B0C86F0A10ECFD0A1CE266BFAAF1FCFC0ABB5C9BC8` |
| `Assets/UI/Shaders/Hub.meta` | `12819597576562f134f4225299f186fe` | `folderAsset: yes`, empty `DefaultImporter` fields | 172 | `0441D34AA04ED1622960D9B115A249E452A1E75B97DB0E1DCF8D8A861EAAE5A9` |
| `TMP_SDF-Mobile.shader.meta` | `5d56ff4c414a9d872126e38007c1d57d` | `ShaderImporter`, empty external/default/non-modifiable/userData/bundle fields | 204 | `94D3127E88D565DE1A4D71F6FB8F4E08A2CAEAC3D2FD6D8BE0277C033E9BF91C` |
| `TMPro_Properties.cginc.meta` | `f0cdd71d926cc071f345d0b51adef566` | `ShaderImporter`, same empty fields | 204 | `E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8` |

고정 GUID는 upstream GUID를 재사용하지 않는다. 나중에 금지된 Essential
Resources 전체가 실수로 임포트되더라도 upstream GUID 충돌로 조용히 덮어쓰는
상황을 피하고, validator가 duplicate shader name과 금지 경로를 명시적으로
거부하게 하기 위해서다.

## Builder 생명주기 경계

`HubPresentationAuthoringBuilder`는 이 자산들을 생성·복사·수정·삭제하거나
`AssetDatabase.ImportPackage`를 호출하지 않는다. 모두 source-controlled
precondition이다.

SDF 두 face 중 하나라도 만들기 전에 validator가 다음을 모두 통과해야 한다.

1. uGUI package declaration과 resolved version이 정확히 2.6.0이다.
2. 두 source asset과 네 meta, package notice가 exact path/GUID/importer/hash다.
3. shader source의 선언 이름과 include 표면이 위 고정값이다.
4. `AssetDatabase.LoadAssetAtPath<Shader>(canonicalPath)`가 non-null이다.
5. `Shader.Find("TextMeshPro/Mobile/Distance Field")`가 바로 그 canonical asset과
   reference-equal이며 같은 이름의 두 번째 project asset이 없다.
6. shader compiler error가 0이고, `Assets/TextMesh Pro/` 및 Essential Resources
   package import 흔적이 없다.

preflight 실패 시 SDF, atlas, material, prefab, scene을 쓰지 않고 fail closed한다.
builder가 SDF를 생성하는 동안 shader/include/license/meta는 immutable이다.
생성된 Regular/Bold SDF material은 각각 canonical shader를 직접 참조해야 한다.
glyph population 후 `Static`, multi-atlas false, fallback empty인 기존 계약은
그대로다.

예외 시 builder가 정리할 수 있는 것은 그 invocation에서 아직 저장되지 않은
in-memory TMP/font/texture/material 객체와, 해당 invocation이 새로 만들다 실패한
두 SDF target뿐이다. canonical shader/include/settings/font/license나 이미 유효한
prefab/scene을 repair·재작성하지 않는다. same-process와 fresh-process 재실행은
shader/include/license/meta 및 완성된 SDF/prefab/scene bytes를 모두 보존하는
no-op이어야 한다.

## Validator와 반례 행렬

observational validator와 독립 tests는 최소한 다음 변이를 각각 거부해야 한다.

- shader/include/license 또는 네 meta의 missing, rename, byte drift;
- 각 고정 GUID, importer type, canonical meta schema/hash drift;
- shader 선언 이름 변경, include 누락/추가/변경, compiler error;
- `Shader.Find` null, 다른 path asset, duplicate shader name;
- `Assets/TextMesh Pro/` 또는 Essential Resources 전체/부분 추가 import;
- uGUI version/fingerprint drift 또는 copied source/evidence constant mismatch;
- 생성 SDF material의 null/foreign shader, canonical shader reference 불일치;
- package default/fallback font, dynamic population, runtime glyph addition,
  multi-atlas, 추가 TMP Resources 또는 추가 TMP shader 도입;
- builder pass 전후 canonical shader/include/license/meta byte 변경;
- failed preflight 뒤 SDF/prefab/scene 또는 다른 allowlist asset 생성·변경.

validator는 고치지 않는다. 런타임 reflection, TMP private static injection,
package cache 경로에 대한 runtime 의존, 자동 Essential Resources importer를
추가하지 않는다. archive/member hash 재계산은 acquisition/implementation
evidence 단계에서 한 번 수행하며, builder·player·완성 asset이 `Library/PackageCache`
경로에 의존하지 않는다.

## 계약 delta

계약은 구현 재개 전 잠시 `Review`로 내려 다음을 반영해야 한다.

- `REQ-M5D7QA-007`: 검증된 Noto 정적 SDF뿐 아니라 위 exact uGUI 2.6.0 Mobile
  SDF shader pair와 Companion License notice를 직접 참조하도록 확장한다.
- `REQ-M5D7QA-009`: 두 source shader files와 notice/meta를 immutable
  precondition/observational-validation 범위에 추가한다.
- `AC-M5D7QA-002`: same/fresh builder byte preservation 및 위 독립 shader
  provenance/path/GUID/hash/duplicate/import 변이를 추가한다.
- `AC-M5D7QA-008`: package/archive/member/source/meta/license hashes,
  `Shader.Find` identity, compiler-error zero, 두 SDF material의 canonical shader
  직접 참조를 증명한다.
- `AC-M5D7QA-010`: focused EditMode에 위 mutation matrix를 포함하고 PlayMode/
  capture에서 실제 Korean TMP render material에 missing/error shader가 없음을
  확인한다.
- Exact allowlist와 rollback에 위 자산·증거·register row만 추가한다.
- 기존의 “TMP Essential Resources 금지”는 **전체 bundle/default resources 자동
  import 금지**로 유지하되, 위 두 exact provenance files만 명시적 예외로 쓴다.

기존 result stem으로 이 검증을 수행할 수 있으므로 새 영구 결과 파일명은
필요 없다. `font-package-smoke`, `builder-pass-a/b`, `builder-fresh-process`,
`scope-audit`, `focused-editmode`, `focused-playmode`, `final-manifest`에 각각
정해진 증거를 넣는다.

## 증거 요구

`AST-UI-SHADER-001.md`와 Terra 구현 증거에는 다음을 기록한다.

- Unity 6000.6.0f1, uGUI 2.6.0, package fingerprint와 lock/manifest 위치;
- Essential Resources archive size/hash 및 두 upstream member path/GUID/size/hash;
- target source asset/meta/license size/hash와 AssetDatabase GUID/path;
- package `LICENSE.md` hash, 공식 Unity Companion License URL, 프로젝트 사용
  범위와 notice 보존 위치;
- `Shader.Find` canonical identity, shader name, compiler message count;
- Regular/Bold SDF asset/atlas/material hashes와 canonical shader GUID/reference;
- full bundle 미임포트, default/fallback/dynamic/runtime path 부재;
- builder 전후 및 fresh-process byte manifest;
- 해당 `REQ-*`와 `AC-*`, Terra 구현·Luna 독립검증·Astra 최종판정.

## 승인 권고

이 안은 shader lookup을 만족하는 최소 영구 표면이다. shader 한 파일만 복제하면
직접 include가 끊기고, full Essential Resources를 임포트하면 기본 폰트·fallback·
style/line-breaking/다수 shader까지 들어와 기존 폐쇄 계약을 무너뜨린다. 정확한
두 source file + notice가 양쪽 극단을 피한다.

Luna가 package source, 해시, 라이선스 고지, mutation matrix를 좁게 재검토해
`P0=0/P1=0`을 확인하면 Astra가 계약에 delta를 반영하고 `Approved`를 복원할 수
있다. 그 전까지 Terra 구현과 Unity 재실행은 중단 상태를 유지한다.
