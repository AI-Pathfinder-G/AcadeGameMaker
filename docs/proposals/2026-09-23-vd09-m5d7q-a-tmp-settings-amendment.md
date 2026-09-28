# M5D7Q-A 최소 TMP Settings 보정 제안

- 날짜: 2026-09-23
- 설계 검토: Sol (`gpt-5.6-sol`)
- 결정 권한: Astra
- 상태: 계약 보정 제안
- 원인 증거: `artifacts/unity-results/m5d7qa-20260923/builder-pass-a.log`

## 문제

uGUI 2.6.0의 공개 `TMP_FontAsset.CreateFontAsset` 경로는 atlas population
mode와 무관하게 `TMP_Settings.clearDynamicDataOnBuild`를 읽는다. 프로젝트에
`Resources/TMP Settings.asset`이 없으면 `TMP_Settings.instance`가 null이고, 실제
M5D7Q-A builder는 정적 atlas 생성 전에 `NullReferenceException`으로 중단된다.

기존 계약은 default/fallback/dynamic font와 TMP package resource import를
금지했지만, 그 문구가 TMP 런타임 설정 객체 자체까지 금지하는 것으로 작성되어
공개 API의 필수 선행조건과 충돌한다. 인메모리 또는 실행 중 임시 설정은 Resources
singleton을 domain/runtime reload 뒤 유지하지 못하므로 유효한 해결이 아니다.

## 최소 보정

정확히 하나의 프로젝트 소유 설정 자산
`Assets/UI/Resources/TMP Settings.asset`과 고정 `.meta`만 허용한다. 이는 기본
폰트 자산이나 TMP Essential Resources 묶음이 아니다.

설정 자산은 다음 닫힌 상태를 갖는다.

- type은 uGUI 2.6.0의 정확한 `TMPro.TMP_Settings`이다.
- `assetVersion`은 `2`다.
- `Clear Dynamic Data on Build`는 `true`다.
- runtime font-feature retrieval은 `false`다.
- active font-feature list는 null이 아닌 empty list다.
- `useModernHangulLineBreakingRules`는 `true`다. 따라서 정확한 한국어 문자열의
  `Normal` wrapping은 null line-breaking TextAsset을 역참조하지 않는다.
- default font, fallback fonts, default sprite, emoji fallback, default style sheet,
  line-breaking text assets는 모두 null/empty다.
- default font/sprite/style/color-gradient resource path는 모두 empty다.
- 다른 TMP asset, package resource import, OS font, runtime fallback 또는 dynamic
  atlas authority를 추가하지 않는다.

허브의 모든 TMP component는 계속 검증된 Regular/Bold 정적 SDF를 직접 참조한다.
설정 자산은 누락된 폰트를 보충하거나 텍스트를 대체할 권한이 없다.

## Authoring 경계

설정 자산은 source-controlled canonical YAML과 고정 meta GUID로 제공한다.
`HubPresentationAuthoringBuilder`는 이를 생성·수정·삭제하지 않고 SDF 생성 전에
정확히 하나가 `Resources.Load<TMP_Settings>("TMP Settings")`로 로드되는지 확인한다.
동일 이름의 두 번째 Resources asset은 실패다.

검증기는 uGUI package version과 TMP_Settings script GUID, exact path/GUID,
`assetVersion=2`, modern Hangul=true, non-null empty active-feature list, 위
null/empty/boolean 정책 및 다른 TMP Resources 부재를 확인한다.
이 검사는 package-version-locked settings YAML schema에 한정한다. 런타임 reflection,
프로젝트 component의 private-field string, 자동 package resource importer는 여전히
금지한다.

SDF 생성은 편집기 메모리에서만 `Dynamic`으로 시작해 정확한 138 glyph를 넣은 뒤,
자산 저장 전에 `Static`으로 동결한다. 최종 저장물은 source font GUID와 생성
settings evidence를 보존하되 runtime glyph addition, fallback, multi-atlas를 갖지 않는다.

## 계약·검증 delta

Allowlist에 다음만 추가한다.

- `Assets/UI/Resources.meta`
- `Assets/UI/Resources/TMP Settings.asset` and `.meta`

`AC-M5D7QA-002/008`은 missing, duplicate, wrong type/GUID/version, default/fallback
reference, non-empty resource path, runtime-feature retrieval, null/non-empty active
feature list, modern Hangul=false, clear-dynamic=false 및 추가 TMP Resources 각각을
독립적으로 거부해야 한다. same/fresh-process builder는 설정 asset/meta bytes를
바꾸지 않아야 한다.

## 결론

이 보정은 사용자 경험·한국어 copy·폰트·atlas·입력·장면 범위를 바꾸지 않는다.
사용자 결정은 필요 없다. Luna가 이 좁은 보정을 재검토하고 Astra가 다시
`Approved`로 전환한 뒤 Terra가 같은 구현을 재개할 수 있다.

