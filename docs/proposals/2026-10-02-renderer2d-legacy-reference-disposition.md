# Renderer2D 일곱 과거 참조의 처리 설계 초안

- 상태: **Draft, 제한 정적 설계.** 작성 역할은 실제 `gpt-6-sol`이다. 자산·소스·설정·패키지 변경, Unity/Git/네트워크 실행, 기존 P1 폐쇄 또는 독립 수용을 수행하지 않았다.
- 추적: `REQ-PLAT-047/044/045/046`, `AC-PLAT-045/042/043/044`. v2 로드 관측（`../../artifacts/c4-renderer-guid7-loaded-probe-v2.json`；로컬 보존 자료） SHA-256 `A42A5E41E3D83DBE89B5AE51A95975F8C3EC378CE0BFCA217B310350462699F8`과 [기존 루나 평가](../verification/2026-09-29-c3-renderer2d-guid7-luna-review.md) SHA-256 `236CFE29EC0BE611993991AF8007F93D2F61E31EE9D620B51D1246F5C19907D5`를 구분한다. v2는 `output/u505-r1`에서 Unity `6000.6.0f1`을 한 번 실행한 관측이며 루나 독립 수용은 아직 없다.

## 현재 사용과 과거 직렬화의 정확한 차이

`Assets/Settings/Renderer2D.asset` SHA-256 `B8F2684833BF219E0348E5D2EA193AC2F3CC8481755BDC75206DFE6750DC6368`의 일곱 줄은 그대로 남아 있다. v2에서 이들 GUID의 경로는 모두 빈 값이다. 20–25행의 여섯 `probeVolumeResources`는 로드 객체의 속성이 존재하고 `ObjectReference`이지만 실제 객체는 `null`이다. 64행의 `m_FallOffLookup`은 `FindProperty` 결과가 없어 타입·참조·실제 대상이 **미관측**이다. 아래 GUID와 줄은 자산 YAML의 역사 값이며 현재 대상 GUID로 해석된 값이 아니다.

| 저장 필드 | YAML 줄·GUID | 현재 로드 관측 |
| --- | --- | --- |
| `probeVolumeDebugShader` | 20, `e5c6678ed2aaa91408dd3df699057aae` | 속성 존재·참조 null |
| `probeVolumeFragmentationDebugShader` | 21, `03cfc4915c15d504a9ed85ecc404e607` | 속성 존재·참조 null |
| `probeVolumeOffsetDebugShader` | 22, `53a11f4ebaebf4049b3638ef78dc9664` | 속성 존재·참조 null |
| `probeVolumeSamplingDebugShader` | 23, `8f96cd657dc40064aa21efcc7e50a2e7` | 속성 존재·참조 null |
| `probeSamplingDebugMesh` | 24, `57d7c4c16e2765b47a4d2069b311bffe` | 속성 존재·참조 null |
| `probeSamplingDebugTexture` | 25, `24ec0e140fb444a44ab96ee80844e18e` | 속성 존재·참조 null |
| `m_FallOffLookup` | 64, `5688ab254e4c0634f8d6c8e0792331ca` | 속성 부재·대상 미관측 |

설치된 `output/u505-r1/Library/PackageCache/com.unity.render-pipelines.universal@8457e85b8184`의 `package.json`은 `17.6.0`(SHA-256 `16D92769D5B472D3A06BA4D80D6734ADED7835265D0B875E2ACE3D976EFD6DF6`)이다. 그 `Runtime/Deprecated.cs` SHA-256 `4528F725DAB168971D0E4703E4F3D3C5CC8C67C704E4432AA000E44D239D5CEF` 254–305행은 여섯 필드를 과거 `ProbeVolumeResources`로 선언하고 현재 `ProbeVolumeDebugResources`로 이동했다고 표시한다. 현재 Core `ProbeVolumeGlobalSettings.cs` SHA-256 `27FF6A287C3B8E4C7D979CFCD5964EA2E94B341E6C32407D70396A6FF88D250F` 26–46행은 새 디버그 자원과 `ResourcePath`를 선언한다. `ProbeReferenceVolume.Debug.cs` SHA-256 `499B65DE58FB4E1EA2E418D3964CD0A6C0A84F04BBB12373D60FAEAB04459147` 381–406행은 `PROBEREFERENCEVOLUME_DEBUG` 조건에서 `GraphicsSettings.TryGetRenderPipelineSettings<ProbeVolumeDebugResources>`로 이를 얻어 디버그 재료·메시·텍스처에 사용한다. 이는 현재 코드의 **조건부 자원 공급 경로**이지 과거 여섯 GUID가 해석됐다는 증거가 아니다.

프로젝트 `Assets/Settings/UniversalRenderPipelineGlobalSettings.asset` SHA-256 `75AC0247F483990A85E147DF1EC52C50C63F489C601E2C9B053570794982E288` 451–459행에는 그 새 자원 여섯 개가 **다른 GUID**로 직렬화돼 있다. 현재 Core PackageCache의 각 대응 `.meta`에서 그 여섯 GUID가 각각 한 번 발견됐다. `ProjectSettings/GraphicsSettings.asset` SHA-256 `BCDB6EE570B5EDC137B79E48F860495035136E494030E214FE39C151D16FE993` 60–61행은 이 전역 설정 자산 GUID에 결속되고, `ProjectSettings/QualitySettings.asset` SHA-256 `7EC339D3E5E2B8CC73548E69256F6151782BB68A4AFB64F865FD94704DBF0DE3` 51행 등은 `Assets/Settings/UniversalRP.asset`에 결속된다. 이 연결은 현재 프로젝트의 정적 구성 근거이며 실제 클린 복제의 import·렌더 성공은 증명하지 않는다.

일곱째는 현재 URP `Renderer2DData.cs` SHA-256 `A88EAFDE3FA0E770BDBD0396443833B8357C1727A14C85CFBCAE940203DCF645` 94–103행에서 렌더러를 생성할 때 이전 저장 텍스처를 직접 읽지 않는다. 현재 `Light2DLookupTexture.cs` SHA-256 `A9DEFB3B7004EA0687136CFDFFC3852BEAE6F88F1D73896FD53AD7754D349C18` 36–46·111–120행은 falloff 텍스처를 생성·보관하고, `GlobalPropertiesPass.cs` SHA-256 `6B8422BA3D38E490203DD1DF6F8A1A4727DD524CD6C447E662EE34D9E8D60CEF` 28–35행이 이를 렌더 그래프 전역 텍스처로 공급한다. 그러나 자산의 `m_FallOffLookup` 원래 형식·대상과 이 경로의 완전한 대체 관계는 입증되지 않았다. 편집기에서는 `Renderer2DData.Create`가 null 재적재를 부르고(97–100행), `Renderer2DDataAuthoring.cs` SHA-256 `89A424C9343ED51D7CBA51B9E3AB93C488558FA5316B0D4096BCFC7B77F8A786` 75–80행도 자원 재적재를 부른다. 따라서 정적 검색만으로 과거 필드가 어떤 경로에서도 영향을 주지 않는다고 단정하지 않는다.

`Packages/manifest.json` SHA-256 `AD3A5D2329D5A8FD71BE89873C75BA2CAA98B4391558A69D83D81CADFE684E1E` 5행의 URP 요청은 `17.3.0`, `Packages/packages-lock.json` SHA-256 `B2B9B1DE4381F3C289837ACD1BDE69013C1F7581511E26861DFDF4F5AE00E550` 66–74행과 현재 PackageCache는 `17.6.0`이다. 기존 루나 검토대로 이 표기 차이를 새 결함이나 무단 업그레이드로 판단하지 않는다.

## 처리 후보와 후속 검증

| 후보 | 영향과 필요한 근거 |
| --- | --- |
| **현 자산 무변경 유지** | 역사 YAML과 현재 프로젝트 바이트를 보존한다. 현재 로드값 여섯 null·한 속성 부재, 새 Core 자원/생성 falloff 경로만으로 **무영향·의존 폐쇄·게시 적합**을 주장할 수 없다. 독립적으로 동일 버전 클린 복제의 import·실제 Renderer2D 사용·APV 디버그/2D 조명·falloff 경로와 누락 자원 진단을 확인해야 한다. 그 전 P1은 열린다. |
| **현재 버전의 정식 자산 재생성·정리** | 별도 Approved 변경 계약에서 정확 `Renderer2D.asset` 및 필요 시 `.meta`·`UniversalRP.asset` 참조·전역 설정·`ProjectSettings`의 허용 경로를 먼저 고정해야 한다. 현재 URP `Renderer2DData` 생성·직렬화 절차로 새 후보를 만들고 일곱 필드의 실제 삭제/치환 여부를 바이트 차이로 확인한다. 평범한 저장만으로 과거 필드가 사라진다고 가정하지 않는다. GUID·renderer 인덱스·scene/prefab 결속, 2D 조명/그림자·APV 디버그/생성 falloff, 설정·패키지 지문 및 클린 복제 실행을 기존 값과 비교해야 한다. 제품 시각 결과나 GUID가 달라질 수 있으므로 원래 501/505 기준을 자동 갱신하지 않는다. |

**다음 한정 계약 권고:** 먼저 자산 무변경의 격리된 클린 복제 진단을 별도 승인하되, 정확 Unity `6000.6.0f1`·URP/Core `17.6.0`·501/505 전후 지문을 동결한다. 로드된 `Renderer2DData`의 일곱 속성, 전역 `ProbeVolumeDebugResources` 여섯 실제 대상/경로·GUID, 생성 falloff 텍스처의 호출 여부와 2D 렌더 결과, import/콘솔의 누락 참조·컴파일·패키지 진단을 제한 출력으로 남긴다. 판정은 **현재 사용 경로 관측**과 **일곱 역사 GUID의 해석/무영향 여부**를 분리한다. 결과가 모호하거나 역사 필드가 문제를 일으키면 그때 정확 자산 변경 계약을 새로 심사한다. 이 제안 자체는 Unity 실행·자산 재생성 승인이 아니다.

일곱 저장 GUID는 모두 경로 미해석이며 현재 P1은 그대로다. v2의 `PublicationApproved=false`, `DependencyClosure=false`, `WholeAccepted=false`, `CleanCloneExecuted=false`와 루나 미검수 상태를 보존한다. 독립 루나 검수와 아스트라의 별도 기술 판정 전에는 어느 후보도 게시·전체 기반 수용에 사용하지 않는다.
