# Renderer2D 일곱 역사 참조의 현행 범위 처분 계약 초안

상태: **Draft**. `REQ-PLAT-077/078`, `AC-PLAT-075/076`은 작성 시 문서 검색에서 미사용인 제안 번호다. 이 문서는 원본 `Renderer2D.asset` 수정, 일곱 GUID 해석 성공, 과거 P1 자동 폐쇄 또는 전체 이식·게시 승인을 뜻하지 않는다.

## 정적·실행 경계

`Assets/Settings/Renderer2D.asset` SHA-256 `B8F2684833BF219E0348E5D2EA193AC2F3CC8481755BDC75206DFE6750DC6368`의 여섯 `probeVolumeResources` 참조(20–25행)와 `m_FallOffLookup`(64행)는 현재 설치 `Assets/Packages/PackageCache` 메타에서 같은 GUID가 발견되지 않았다. 로드된 실제 `Renderer2DData`의 여섯 속성은 `ObjectReference`지만 값이 null이며 일곱째 속성은 조회되지 않았다(`artifacts/c4-renderer-guid7-loaded-probe-v2.json` SHA-256 `A42A5E41E3D83DBE89B5AE51A95975F8C3EC378CE0BFCA217B310350462699F8`). 이는 로드 객체의 관측값이지 오래된 직렬화 원문이 제거됐다는 뜻이 아니다.

설치 URP 17.6.0의 `Runtime/Deprecated.cs` SHA-256 `4528F725DAB168971D0E4703E4F3D3C5CC8C67C704E4432AA000E44D239D5CEF` 253–305행은 여섯 필드를 `ScriptableRendererData`의 구형 APV 자원으로 선언하고 새 `ProbeVolumeDebugResources`로 이동했다고 명시한다. 현재 `Runtime/2D/Renderer2DData.cs` SHA-256 `A88EAFDE3FA0E770BDBD0396443833B8357C1727A14C85CFBCAE940203DCF645`와 `Renderer2DDataAuthoring.cs` SHA-256 `89A424C9343ED51D7CBA51B9E3AB93C488558FA5316B0D4096BCFC7B77F8A786`에는 `m_FallOffLookup` 선언/읽기가 없고, 편집기 경로는 `ReloadAllNullProperties`를 호출한다(앞 파일 94–103행, 뒤 파일 75–81행). 실제 패키지 C#의 `probeVolumeResources` 참조는 위 Deprecated 선언뿐이지만, 이 검색만으로 모든 플랫폼·플러그인·셰이더 동적 사용을 부정하지 않는다.

현행 공급 경로는 별도다. `Runtime/RenderPipelineResources/Renderer2DResources.cs` SHA-256 `BA35B2B64C74C063747563CDFC80B03204F723578E83298E2E2646ADF51C1048`의 현재 `[SerializeField, ResourcePath]` 재질·조명 셰이더 필드(10–105행), `Runtime/2D/Passes/Utility/Light2DLookupTexture.cs` SHA-256 `A9DEFB3B7004EA0687136CFDFFC3852BEAE6F88F1D73896FD53AD7754D349C18` 36–46·111–120행의 새 falloff 텍스처 생성, `Runtime/2D/Rendergraph/Renderer2DRendergraph.cs` SHA-256 `2D5B4C7CF2EFE7B114E2EC83A644979F6B0F6913C5E1556ABA18AE2E128B8833` 929–934행 및 `GlobalPropertiesPass.cs` SHA-256 `6B8422BA3D38E490203DD1DF6F8A1A4727DD524CD6C447E662EE34D9E8D60CEF` 28–35행의 조명 조건부 결속으로 이어진다. 별도 Core 17.6 `ProbeVolumeGlobalSettings.cs` SHA-256 `27FF6A287C3B8E4C7D979CFCD5964EA2E94B341E6C32407D70396A6FF88D250F` 27–48행은 새 APV 디버그 자원 여섯 개를 `[ResourcePath]`로 선언하며, `ProbeReferenceVolume.Debug.cs` SHA-256 `499B65DE58FB4E1EA2E418D3964CD0A6C0A84F04BBB12373D60FAEAB04459147` 384–405행의 소비는 `PROBEREFERENCEVOLUME_DEBUG` 조건 아래다. 여섯 새 객체는 현재 전역 설정에서 관측됐지만 디버그 기능 실제 실행까지 확인한 결과는 아니다. v6 격리 진단은 정상 `LateUpdate` 뒤 점 조명 켜짐/꺼짐에서 RGBA 16픽셀 차이를 실제 관측했다(`docs/verification/2026-10-02-renderer-normal-update-v6-terra-report.md` SHA-256 `14FD0694C62D39702ECF7E15D6BE237DA5A90B8CBD4889E791C30E747A8FC634`). 이는 현재 편집기 미리보기의 점 조명 효과이며 내부 lookup 샘플의 직접 계측은 아니다.

설치 Unity `6000.6.0f1`의 `UnityEditor.xml`과 `UnityEngine.xml` 정적 탐색에서 패스별 RenderGraph 텍스처 결속을 반환하는 **공개** 프레임 디버거 조회 API는 확인되지 않았다. `UnityEngine.FrameDebugger.enabled`는 상태 조회일 뿐 이벤트·자원 증거가 아니다. 따라서 존재를 확인하지 못한 API 이름으로 진단 구현을 승인하지 않는다. 외부 콜백 시점 `Shader.GetGlobalTexture`가 `UnityBlack`인 결과도 패스 내부 부재의 증거가 아니다.

## 최소 후속 판정

제안 `REQ-PLAT-077` / `AC-PLAT-075`: 독립 정적 원장은 실제 설치 URP/Core 버전·패키지 캐시 원문 SHA와 컴파일 조건을 고정하고, `Renderer2DData`의 모든 실제 partial 선언·상속 `ScriptableRendererData`의 구형 여섯 필드·`m_FallOffLookup` 부재·현재 새 자원 경로·모든 참조 검색 위치를 정확 줄/파일로 열거한다. 실제 로드 객체 일곱 속성 결과, 활성 URP/Renderer2D와 새 전역 APV 자원 관측, v6의 두 실제 렌더 원시 지문을 각각 별개 근거로 결속한다. 검증 대상은 **Unity 6000.6.0f1 + 현 패키지 17.6.0 + 현재 격리 미리보기의 기본 Sprite-Lit/점 조명**이다. 조명 효과와 역사 필드 비소비 판단을 혼합하지 않는다.

제안 `REQ-PLAT-078` / `AC-PLAT-076`: 루나가 위 원장을 독립적으로 원문·행·SHA와 대조하고 아스트라가 새 Approved 한정 처분에서 ‘이 현행 지원 표면의 Renderer2D 조명 경로는 역사 일곱 GUID의 실제 객체 해석을 필요로 했다는 증거가 없다’는 범위까지만 판정한다. 실제 코드가 역사 필드를 읽는 분기, 새 패키지/플랫폼, APV 디버그 화면, 전역 설정 이외의 렌더러, 렌더 패스의 직접 lookup 소비, 클린 복제·빌드·게시 이식성은 별도 미입증이다. 이후 정확한 현재 지원 범위에 대해 P1을 부분 종결할지 결정할 수 있으나, 일곱 GUID 원문 대상 미해석과 최초 코드 게시 차단을 이 문서만으로 풀지 않는다.

기존 C3/C4의 최종 아홉 실행은 새 게임 초기화 권한·디스크/메모리 가드와 188+377 필수 행을 검증한 범위다(`docs/approvals/2026-10-01-c3-c4-v15-joint-final-integration-acceptance.md`). 그 수용을 Renderer2D 일곱 참조/조명 셰이더의 직접 시험으로 재표기하지 않는다. 원본 asset과 과거 검수 지문, 현재 후보/원장, 설치 패키지 소스는 보존한다. 완전한 게시 판단에는 ADR-0038의 파일별 출처·의존 및 깨끗한 복제 관문이 계속 필요하다.
