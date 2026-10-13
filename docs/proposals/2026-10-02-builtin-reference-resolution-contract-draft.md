# Unity 내장 참조 후보 19행의 객체 확인 계약 초안

상태: **Draft**. 이 문서는 조사·진단 구현·실행 승인이 아니다. 제안 식별자 `REQ-PLAT-071/072`, `AC-PLAT-069/070`은 작성 시 기존 문서 검색에서 미사용이었다. 번호 확정은 아스트라 승인 때 한다.

## 현재 증거와 한정 범위

`artifacts/c4-unresolved-reference-classification-v1.json` SHA-256 `79ABC17E2A8E4B6BBAE8447BCE6838CC01C43967026DA68DE17A2584B4E61262`의 `MissingGuidValues`는 아래 두 값이 `Assets`·`Packages`·패키지 캐시의 메타 파일에서 발견되지 않았다고만 기록한다. 이는 내장 객체 판정이나 결손 판정이 아니다. 현행 소유 분류 계약 `docs/specs/work-contracts/2026-10-02-unresolved-reference-owner-classification.md` SHA-256 `5BAAAC79F141226C27E748DD8DBC8EBF8C9B31F93156BC86943A5D3726E6C21A`의 미폐쇄 상태를 유지한다.

| GUID | 실제 출발점·위치·직렬화 필드 | 원문 `fileID` | 출발 파일 SHA-256 |
| --- | --- | --- | --- |
| `0000000000000000e000000000000000` | `Assets/Scenes/Bootstrap.unity:34`, `RenderSettings.m_SpotCookie`; `Assets/Scenes/Hub.unity:34`, 같은 필드 | 두 행 모두 `10001`, `type: 0` | 차례로 `EB6809DE2B02518BBE693FB771F6B61B154133E07C693F48465C8DF5DB05C0A9`, `04F3F0EA73C40A964DCE93766CF21AFE3F57DC20912AEAB86ABDADB239BC9C6A` |
| `0000000000000000f000000000000000` | `Assets/Scenes/Hub.unity:29` `RenderSettings.m_SkyboxMaterial`, `:96` `LightmapSettings.m_LightingDataAsset` | `10304`, `20201`; 모두 `type: 0` | `04F3F0EA73C40A964DCE93766CF21AFE3F57DC20912AEAB86ABDADB239BC9C6A` |
| 같은 `f` 값 | `ProjectSettings/GraphicsSettings.asset:9,12,15,18,21,24,27`의 `m_Deferred`, `m_DeferredReflections`, `m_ScreenSpaceShadows`, `m_DepthNormals`, `m_MotionVectors`, `m_LightHalo`, `m_LensFlare` 각 `m_Shader` | 순서대로 `69,74,64,62,75,105,102`; 모두 `type: 0` | `BCDB6EE570B5EDC137B79E48F860495035136E494030E214FE39C151D16FE993` |
| 같은 `f` 값 | 같은 파일 `:30–36` `m_AlwaysIncludedShaders[0..6]`, `:39` `m_SpritesDefaultMaterial` | 순서대로 `7,15104,15105,15106,10753,10770,10783,10754`; 모두 `type: 0` | 같은 SHA |

따라서 `e` 2행, `f` 17행, 총 19행이다. 나머지 일곱 GUID는 `Renderer2D.asset`의 별도 P1로 남긴다. 19행의 조회 성공도 501개 후보 전체 의존 폐쇄와 게시 허가를 뜻하지 않는다.

## 설치 API에 근거한 제안 검사

설치된 Unity `6000.6.0f1`의 `Editor/Data/Managed/UnityEditor.xml` SHA-256 `A7479F4EFD196F1A709B3370B3D0B217FABEAE3C2A145F03FE97F6DA448E6AD8`는 `AssetDatabase.GUIDToAssetPath`(5292행), `LoadAllAssetsAtPath`(5702행), `TryGetGUIDAndLocalFileIdentifier(Object, out string, out long)`(6095행), `SerializedProperty.objectReferenceValue`(68324행), `EditorSceneManager.OpenScene`(57118행)을 문서화한다. `UnityEngine.xml` SHA-256 `DB0823201DD67DF1007E98FFD84C3F33C1613086E3EB21F2B0B7231EFEC271F7`는 `RenderSettings.skybox`(98727행)를, 전자 문서는 `Lightmapping.lightingDataAsset`(39114행)을 문서화한다. 기존 격리 진단 `output/u505-r1/Assets/Editor/PublicationRendererGuidProbe.cs` SHA-256 `1D572AAEC5E693E949F0BA5AF847DDA1020434270C3E70FF37823ACF601068C0`도 실제 객체의 GUID·파일 식별자 조회 패턴을 사용한다. `m_SpotCookie`의 공개 속성은 이 설치 API 문서에서 확인되지 않았다.

제안 `REQ-PLAT-071`: 별도 승인 후 **격리된 `output/u505-r1`에서만** 편집기 진단을 실행한다. 사전 입력 3파일·분류 JSON·진단 소스·Unity 버전의 경로와 SHA를 고정한다. 19행 각각에 원문 `From/Location/field/index/GUID/fileID/type`을 다시 결속한다. `GUIDToAssetPath`가 돌려준 실제 경로가 있으면 `LoadAllAssetsAtPath`로 객체를 열거하고, 각 실제 객체에 `TryGetGUIDAndLocalFileIdentifier`를 적용한다. 경로가 비거나 호출이 실패하면 내장으로 추정하지 않는다. 장면 두 개는 격리 프로젝트에서 `OpenScene`으로 읽고 `RenderSettings.skybox`·`Lightmapping.lightingDataAsset`의 실제 객체가 있는 경우 동일 조회값을 원문과 대조한다. `m_SpotCookie`와 직접 공개 읽기 경로가 확인되지 않은 그래픽 설정 필드는 원문 튜플과 **실제 조회된 유일 객체 튜플**의 결속까지만 판정한다. 객체 이름·형식·`Shader.Find` 결과 또는 유사한 이름은 정확 식별자의 대체 증거가 아니다.

제안 `AC-PLAT-069`: 19행 모두에 조회 시도, 실제 객체 유무, 조회 성공 여부, 실제 객체 형식·경로·GUID·`long fileID`, 원문과의 정확 일치 여부를 각각 기록한다. 동일 `(GUID,fileID)`에 여러 객체가 나오거나 출발 필드의 객체가 다른 경우는 모호/불일치로 남긴다. `GUIDToAssetPath` 공백, 공개 필드 접근 불가, 객체 null, API 오류도 개별 미입증으로 남긴다. 특정 내장 저장소 경로·객체 형식은 실제 응답 전에 하드코딩하지 않는다.

제안 `REQ-PLAT-072` / `AC-PLAT-070`: 진단은 원본과 후보 505/534를 저장·재직렬화하지 않는다. 격리 프로젝트의 대상 장면을 열어도 저장 API를 호출하지 않고 원래 장면 구성으로 복귀한다. 사전·사후 대상 원문, 설정, 패키지, 진단 소스와 원장 입력 SHA가 같아야 한다. 진단의 새 JSON 및 짧은 보고서에 Unity 버전·실제 편집기 종료값·오류·19개 행 결과·미입증 수·`DependencyClosure=false`, `PublicationApproved=false`, `RendererGuid7P1Closed=false`를 기록한다. 정확 출력 경로와 진단 소스·메타 허용 목록은 별도 Approved 구현 계약에서 동결한다. 이 초안은 그것을 생성하지 않는다.

`f` 값의 일부가 실제 객체로 확인되더라도 나머지 행의 성공을 대리하지 않는다. 장면 속 공개 속성으로 읽은 값이 null이면 원문 참조의 해석 성공으로 보고하지 않는다. 그래픽 설정의 15행이 객체 튜플로만 확인되면 사용 중 렌더 경로까지 확인했다고 기록하지 않는다. Renderer2D 일곱 GUID의 현재 사용·렌더 진단 및 독립 루나 검수는 별도 관문이다.
