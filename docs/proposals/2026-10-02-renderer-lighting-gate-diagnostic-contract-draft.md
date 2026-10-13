# Renderer2D 조명 관문 v4 진단 계약 초안

상태: **Draft**. 제품·패키지·자산 변경과 Unity 실행을 승인하지 않는다. 제안 식별자 `REQ-PLAT-073/074`, `AC-PLAT-071/072`는 작성 시 문서 검색에서 미사용이었다. 기존 `docs/verification/2026-10-02-renderer-current-use-v3-luna-review.md` SHA-256 `4BBD36280CFBB98B90BC5A866C1E830B6E8E7B9ED7BE4B798FF29B87F586A2AD`의 Renderer GUID7 P1과 부분 관측 판정을 유지한다.

## 실제 호출 경계

격리 설치본 `output/u505-r1/Library/PackageCache/com.unity.render-pipelines.universal@8457e85b8184`에서 다음 원문을 확인했다.

| 원문과 SHA-256 | 직접 확인한 조건 |
| --- | --- |
| `Runtime/2D/Lights/Light2DManager.cs` `698B5629D42BB9620ED6AFFD33D08F277FCF5F6B8A2DB50FA7A7E8D52C721662` 14–48, 117–132행; `Light2D.cs` `183EE4B7FBDC4ADE89A25779D862418B9EF0C142328DB9F1BECB6FA45999B113` 651–666행 | 미리보기 카메라와 조명은 장면 핸들별 목록을 사용하며, `Light2D.OnEnable`이 등록한다. 객체 생성만으로 카메라가 같은 목록을 조회했음은 증명되지 않는다. |
| `Runtime/2D/Lights/Light2DCullResult.cs` `23F4B5776903B5585E9FF8DEB2C9E1FDF002D8DE008C3EAFC72B6A6BF48EF9A1` 18, 60–83, 129–174행 | `useLights`는 배치 `totalLights > 0`; 카메라 목록에서 조명 계층·편집기 장면 표시·전역/점 조명 시야 검사를 거친다. |
| `Runtime/2D/Rendergraph/Renderer2DRendergraph.cs` `2D5B4C7CF2EFE7B114E2EC83A644979F6B0F6913C5E1556ABA18AE2E128B8833` 929–934행; `GlobalPropertiesPass.cs` `6B8422BA3D38E490203DD1DF6F8A1A4727DD524CD6C447E662EE34D9E8D60CEF` 14–35행 | 배치마다 `useLights`를 합쳐 참일 때만 렌더그래프 패스 **뒤에** `_FalloffLookup`을 전역 결속한다. 카메라 시작 콜백·요청 종료 뒤의 전역 읽기는 이 패스 내부 상태의 대체 증거가 아니다. |
| `Runtime/Materials/Sprite-Lit-Default.mat` `A5B48FCE8D00D118149AB2D5CA8690C7458B595DFE9154EBF1BDBFFB74D0E6D1` 8–10행 및 `.meta` `D4CB079F1D198D2581E2CEA8713C761593C431BBF5CC48AF4EA7F34C15FECD18` 2–5행; `Shaders/2D/Sprite-Lit-Default.shader` `1429736DFF787EA67213A3FA96225EAC12E516A5CB67E4D7F97162C8CE1BAC52` 1,25–32행 및 `.meta` `26D26E3D3F4DE88D22F9B156BC0E7C75EE64AFFEB1732F940E456EC8AE8C31FA` 2행 | 실제 재질 GUID `a97c105638bdf8b4a8650670310a4cd3`, 재질의 셰이더 GUID `e260cfa7296ee7642b167f1eb5be5023`, `Universal2D` 패스를 갖는다. `Assets/Settings/Renderer2D.asset:65–68`의 기본 Lit 재질도 이 GUID다. |
| `Shaders/2D/Light2D.shader` `8B026869017311D25557C7D8C3A64B02A903CC854C551477A3D7A14496CEB46B` 58–63, 190–194, 211–224행 | 조명 셰이더에 lookup 샘플링 코드가 있다. 실제 프레임에서 그 패스가 실행됐다는 뜻은 아니다. |

현재 `PublicationRendererCurrentUseProbeV3.cs` SHA-256 `E65766FE371838D78B0B04347C87A681EE6B35BC1B619ADB7FB427DBA88708E7` 156–200행은 미리보기 장면에 카메라·Sprite·전역/점 조명을 놓지만 `SpriteRenderer`의 실제 재질을 확인하지 않는다. v3의 비검정 픽셀과 `UnityBlack`은 조명 미사용, 등록·컬링 실패, 다른 재질 사용, 관측 시점 차이를 가르지 못한다.

## 승인 후보의 최소 진단

`REQ-PLAT-073`: v3 원본·결과를 보존하고 격리 `output/u505-r1`에만 새 v4 편집기 진단 소스와 메타를 별도 Approved 허용 목록으로 추가한다. 미리보기 장면에서 카메라·두 조명의 `scene.handle`, `EditorSceneManager.IsPreviewSceneObject`, `enabled`, 대상 정렬 계층, 카메라 `cullingMask`, 위치·반경을 **읽기만** 해 같은 목록/컬링 조건을 구별한다. 조명 객체는 장면으로 옮긴 뒤 정상 `AddComponent<Light2D>`로 등록하고, 등록 목록이 필요하면 `Light2DManager`의 실제 내부 목록을 **읽기 전용**으로만 관측한다. 내부 메서드 재호출·private 상태 대입·가짜 배치 생성은 허용하지 않는다. 사전검사에서 미리보기 분류나 정상 등록이 불일치하면 렌더 성공으로 해석하지 않고 그 지점에서 미입증으로 분류한다.

`AC-PLAT-071`: `AssetDatabase.LoadAssetAtPath<Material>`을 위 실제 패키지 재질 경로에 적용해 `TryGetGUIDAndLocalFileIdentifier`로 재질 GUID/`2100000`, `material.shader`의 실제 경로·GUID/`4800000`과 `Universal2D` 패스 존재를 기록한다. fixture의 `SpriteRenderer.sharedMaterial`에 바로 이 로드된 재질을 지정하되 메모리 장면 객체에만 적용하고 `SaveAssets`·자산 저장·원본 `Renderer2D.asset` 변경을 하지 않는다. 이름만 비슷한 재질이나 `Shader.Find` 결과는 허용하지 않는다.

`REQ-PLAT-074` / `AC-PLAT-072`: 동일 카메라·Sprite·재질·렌더 요청으로 정상 조명 켜짐과 정상 컴포넌트 비활성화 대조 프레임을 각각 실제 `RenderPipeline.SubmitRenderRequest`로 제출하고 원시 픽셀 배열의 SHA·차이 픽셀 수·색 차이를 기록한다. 점 조명은 Sprite를 실제로 비추는 위치/반경과 유효 계층으로 놓고 전역 조명은 양쪽 비교에서 동일하게 유지하거나 두 조명 상태를 명시한 별도 대조로 분리한다. 실제 조명 켜짐 프레임과 대조 프레임 사이의 유의미한 차이만 **조명 영향 관측**으로 표시한다. 차이가 없거나 렌더 요청이 지원되지 않으면 조명 관문 미입증이다. `useLights`와 패스 내부 `_FalloffLookup` 결속을 직접 관측하지 못하는 한, 픽셀 차이만으로 그 내부 값·lookup 소비·Renderer GUID7 P1 폐쇄를 선언하지 않는다. 패스 수준 관측을 추가하려면 실제 설치 편집기가 제공하는 프레임 진단 수단과 정확한 이벤트/자원 결속 방법을 별도 승인 범위로 먼저 동결한다. 카메라 콜백 또는 요청 뒤 `Shader.GetGlobalTexture`가 `UnityBlack`인 것만으로 실패나 성공을 판정하지 않는다.

실행 계약에는 진단 소스·메타와 원본 자산·장면·패키지·설정의 사전 SHA, Unity `6000.6.0f1` 버전, 정확 명령/출력·종료코드, 사후 동일 SHA, 오류·경고 및 독립 루나 검수를 요구한다. 재질/셰이더 결속·조명 영향이 입증되어도 이는 격리 장면 한정이며 Renderer 일곱 GUID의 역사 직렬화 해석, 모든 지원 렌더 표면, 전체 의존 폐쇄, 게시 허가를 자동 수용하지 않는다.
