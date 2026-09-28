# VD-09 M5D7Q-A Mobile SDF shader amendment — Luna 독립 사전검토

- 검토일: 2026-09-27
- 검토자: Luna (`gpt-5.6-luna`)
- 대상 계약: `docs/specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md`
- 계약 SHA-256: `B1E3D80F358554D29BB534BC1FB0BFC775B60CE5F9B2E2C25BC884248AF31298`
- 대상 제안: `docs/proposals/2026-09-27-vd09-m5d7q-a-tmp-mobile-sdf-shader-amendment.md`
- 제안 SHA-256: `C4A3B19F958B93F99B7A3D5DAC922F84AE4B64A4C4103861433417C344E5970C`
- 범위: Mobile SDF shader pair의 최소성·출처·해시·라이선스·경로/GUID
  경계·builder preflight/immutability·REQ/AC 추적성·TMP Settings/한국어
  Normal wrapping 회귀 점검
- 구현 여부: 구현하지 않음. 프로젝트에 shader/license/evidence 자산이 아직
  생성되지 않은 상태에서 source/package feasibility만 검증함.

## 검증한 설치 출처

Unity `6000.6.0f1`의 설치된 내장 패키지 `com.unity.ugui`는
`Packages/manifest.json`과 `Packages/packages-lock.json`에서 `2.6.0`으로
고정되고, 기존 Unity 결과 로그의 resolved package path는
`com.unity.ugui@23caec89ae27`이다. 계약의 package fingerprint
`23caec89ae2780ae2aa9b14f95a19c03e3dcdf9e`와 일치한다.

실제 설치 파일을 재계산한 결과:

| 출처 | 바이트 | SHA-256 | 결과 |
|---|---:|---|---|
| `Package Resources/TMP Essential Resources.unitypackage` | 804,874 | `26CDEE2072683CB25CEAFA4FAB23C93C35B2F692E5DAF9CA0F33D05C4E163274` | 일치 |
| `LICENSE.md` | 431 | `3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6` | 일치 |

archive를 임포트하지 않고 tar member를 읽어 확인했다. 두 허용 member의
pathname, upstream GUID, 크기와 내용 해시는 다음과 같다.

| 허용 source member | upstream GUID | 바이트 | SHA-256 | 결과 |
|---|---|---:|---|---|
| `Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader` | `fe393ace9b354375a9cb14cdbbc28be4` | 8,074 | `44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4` | 일치 |
| `Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc` | `3997e2241185407d80309a82f9148466` | 2,707 | `66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90` | 일치 |

shader source는 `Shader "TextMeshPro/Mobile/Distance Field"`를 선언하고
`UnityCG.cginc`, `UnityUI.cginc`, 로컬 `TMPro_Properties.cginc`를 포함한다.
따라서 include 파일을 제외한 한 파일 복제는 불완전하고, 그 외 TMP shader나
Essential Resources 전체 임포트는 과잉 범위라는 제안의 최소성 판단이
성립한다.

## 프로젝트 경로와 `.meta` 결정성

제안의 고정 GUID와 canonical LF/UTF-8/no-BOM YAML을 메모리에서 재생성해
바이트 수와 SHA-256을 독립 계산했다. 계약의 값과 모두 일치한다.

| 프로젝트 경로 | GUID | 바이트 | SHA-256 | 결과 |
|---|---|---:|---|---|
| `Assets/UI/Shaders.meta` | `070c509b184d2bca114677820f278a46` | 172 | `142F6FE56DA2CF181D4E55B0C86F0A10ECFD0A1CE266BFAAF1FCFC0ABB5C9BC8` | 일치 |
| `Assets/UI/Shaders/Hub.meta` | `12819597576562f134f4225299f186fe` | 172 | `0441D34AA04ED1622960D9B115A249E452A1E75B97DB0E1DCF8D8A861EAAE5A9` | 일치 |
| `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader.meta` | `5d56ff4c414a9d872126e38007c1d57d` | 204 | `94D3127E88D565DE1A4D71F6FB8F4E08A2CAEAC3D2FD6D8BE0277C033E9BF91C` | 일치 |
| `Assets/UI/Shaders/Hub/TMPro_Properties.cginc.meta` | `f0cdd71d926cc071f345d0b51adef566` | 204 | `E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8` | 일치 |

프로젝트 GUID는 upstream GUID를 재사용하지 않는다. 이로써 나중에 금지된
Essential Resources import가 발생해도 upstream GUID 충돌로 조용히 덮어쓰는
경로를 피하고, validator가 duplicate name/path를 독립적으로 거부할 수 있다.

## 라이선스와 고지

설치 패키지의 431-byte `LICENSE.md`는 다음 저작권·고지를 포함한다.

`uGUI copyright © 2015 Unity Technologies` 및 Unity Companion License URL
(`https://unity3d.com/legal/licenses/unity_companion_license`). 현재 공식
라이선스 페이지는 [Unity Companion License](https://unity.com/legal/licenses/unity-companion-license)
v1.4이며, Unity Engine License와 연결된 authoring/distribution, substantial
portion에 저작권·라이선스 고지를 제공하는 조건을 명시한다. 제안은 설치
package notice의 정확한 사본을
`third_party/unity-ugui-2.6.0-LICENSE.md`에 보존하고 공식 URL과 사용 범위를
증거 문서에 기록하도록 하므로 이 amendment의 고지 경계를 충족한다.

라이선스 사본, source shader/include, `.meta`, 전용 evidence, asset-register
한 행만 allowlist에 추가된다. archive 전체, 다른 package member, package
import 결과물은 저장하지 않는다.

## 계약 경계 및 회귀 점검

- `REQ-M5D7QA-007/009`가 exact shader pair, notice/meta를 직접 참조하고
  source-controlled immutable precondition으로 둔다.
- `AC-M5D7QA-002`가 same/fresh process byte preservation, missing/rename/
  drift, wrong path/GUID/importer/hash/name/include, duplicate shader name,
  compiler error, package-version drift, 금지 import 및 preflight-write를
  독립 mutation으로 요구한다.
- `AC-M5D7QA-008/010`이 archive/member/source/meta/license provenance,
  `Shader.Find` identity, compiler error zero, 두 SDF material의 canonical
  shader reference와 실제 Korean TMP render material의 missing/error 부재를
  요구한다. REQ/AC traceability는 계약 표에 연결되어 있다.
- builder는 shader/include/license/meta를 생성·복사·수정·삭제하지 않고,
  `AssetDatabase.ImportPackage`도 호출하지 않는다. preflight 실패 시 SDF,
  atlas, material, prefab, scene을 쓰지 않으며 validator는 repair하지 않는다.
- 기존 TMP Settings amendment의 `modern-Hangul=true`, non-null empty
  `m_ActiveFontFeatures`, null line-breaking assets, no-default/fallback/path,
  clear-dynamic=true, runtime-feature=false 경계는 변경되지 않는다. 따라서
  exact 한국어 문자열의 `Normal` wrapping이 null line-breaking asset 경로로
  회귀하지 않는다.
- 단일 UI semantic-frame owner, 입력/notification correlation, Q0 무변경,
  resize/capture 및 effect 금지 경계에도 변경이 없다.

## 위험과 후속 증거

현재 저장소에는 허용 shader/license/evidence 자산이 없으므로 다음은 본
사전게이트의 결과가 아니라 Terra 구현 및 Luna 사후검증에서 반드시 증명할
항목이다: 실제 AssetDatabase GUID/path, compiler message count, `Shader.Find`
reference equality, Regular/Bold SDF/atlas/material hash, builder 전후 및
fresh-process byte manifest, full Essential Resources 미임포트, PlayMode에서
실제 한국어 TMP material의 missing/error 부재. 이 미생성 증거를 현재
성공으로 가장하지 않는다.

## 판정

| 항목 | 판정 |
|---|---|
| 최소성·실현 가능성 | PASS — 정확히 두 source file과 직접 include만 필요 |
| archive/member/GUID/hash/path | PASS — 설치 출처와 canonical `.meta` 해시 일치 |
| Unity Companion License notice | PASS — package notice 보존 위치와 공식 URL 고정 |
| builder immutability/preflight | PASS — 계약에 생성·복구·import 금지와 fail-closed가 명시됨 |
| duplicate/forbidden-import mutation coverage | PASS — AC-M5D7QA-002/010에 추적됨 |
| TMP Settings/한국어 Normal wrapping 회귀 | PASS — 기존 modern-Hangul/feature-list closure 유지 |
| P0 | 0 |
| P1 | 0 |
| P2 | 0 (단, 구현 후 증거 미생성은 후속 게이트 항목) |
| 새 사용자 제품 결정 | 불필요 |

**최종 독립 사전게이트: PASS — P0=0, P1=0, P2=0.**

Astra는 이 기술 amendment를 반영한 최신 계약을 `Approved`로 복원할 수
있다. 이는 구현 승인 이후에도 Terra의 source-controlled 자산 작성,
집중/전체 Unity 검증과 Luna 독립 사후검증이 필요하다는 뜻이며, 아직
`Verified` 판정은 아니다.

권위 문서:

- [M5D7Q-A contract](../specs/work-contracts/2026-09-23-vd09-m5d7q-a-authored-hub-shell.md)
- [shader amendment proposal](../proposals/2026-09-27-vd09-m5d7q-a-tmp-mobile-sdf-shader-amendment.md)
- [Terra blocker evidence](2026-09-23-vd09-m5d7q-a-implementation-evidence.md)
