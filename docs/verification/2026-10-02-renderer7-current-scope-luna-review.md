# Renderer2D 역사 참조 현재 범위 독립 검수

승인 계약 `docs/specs/work-contracts/2026-10-02-renderer7-current-scope-independent-review.md`의 REQ-PLAT-077/078, AC-PLAT-075/076을 읽기 전용으로 대조했다. 처분 JSON SHA-256 `97D2784A2E3AF632AB76A6114A4A2C2FA9BAAC21AE88FA249B975548752419E5`, 테라 보고 `B9C52508B8729D5E054E6BB91896931C289D58EBC966D609A65F3757052F4146`가 계약과 일치한다.

## 판정

- **P0: 0. 현재 설치 한정 원장: 독립 대조 충족.** URP/Core 17.6.0 패키지의 두 지정 경로에서 현재 C# 1,354개를 다시 열거했다. 10개 검색어의 32개 결과 모두 기록과 경로·행·문자열·해당 파일 SHA가 일치했고, 전체 검색 결과 집합 차이는 0이다. JSON의 경로 집합 수와 기재된 digest는 계약값과 일치한다. digest 직렬화 규칙은 입력에 없어 집계 digest 자체는 재계산하지 않았다. 지정 정적 원문 12개와 사전/사후 보호·패키지·기존 증거 지문은 현재 파일 기준 불일치 0이다.
- 일곱 역사 행 모두 `Assets/Settings/Renderer2D.asset`의 지정 행·SHA와 일치했고 v2 로드 JSON의 행·GUID·fileID·관측과도 불일치 0이다. 역사 APV 여섯 필드는 로드 객체의 `ObjectReference`지만 null, GUID 대상 경로는 비었다. `m_FallOffLookup`은 로드 속성이 없고 같은 GUID 대상도 미해석이다. 모든 원문 타깃 상태는 미해석이며 현재 비소비를 증명한 행은 없다.
- **열린 P1: 범용 재로드·마이그레이션 및 게시 전체 소비 미확정.** URP `Deprecated.cs:212, 254–305`는 상속 `ScriptableRendererData`의 역사 APV 그룹과 여섯 필드를 `Obsolete`/`#from(2023.3)`으로 보존한다. 직접 C# 검색의 `probeVolumeResources`는 선언 한 곳뿐이고 `m_FallOffLookup`은 0건이다. 그러나 `ResourceReloader.cs:64–75`의 반사 필드 순회, `ScriptableRendererData` 재로드 경로 및 `Renderer2DDataAuthoring.cs:78–80`의 재로드·마이그레이션 가능성이 있으므로 직접 검색 부재는 전 경로 비소비 증거가 아니다. 특정 옛 GUID가 성공적으로 재결속됐다는 관측은 없다.
- 현재 경로는 별도 `Renderer2DResources.cs`, 생성형 `Light2DLookupTexture.cs`, `Renderer2DRendergraph.cs:929–932`의 `useLights` 조건 및 `GlobalPropertiesPass.cs:28–35`의 전역 텍스처 연결로 분리된다. `Light2D.shader:190–224`는 lookup을 샘플링한다. 이는 현재 구현 경로의 정적 증거이지 이 원장의 특정 격리 실행에서 내부 소비를 계측한 결과가 아니다.
- APV 새 자원 6개는 `ProbeVolumeGlobalSettings.cs:27–46`에 존재하고 v3 로드 관측과 일치하지만 `ProbeReferenceVolume.Debug.cs:381–384`의 컴파일/설정 조건부 경로에서 실제 디버그 실행은 관측되지 않았다. v6 격리 Sprite-Lit 점 조명은 정상 갱신 뒤 16픽셀 차이를 기록했으나 `_FalloffLookup`은 UnityBlack이며 직접 lookup 소비는 false다. 이 결과를 APV 실행이나 Renderer GUID7 소비로 일반화하지 않는다.

AC-PLAT-075/076의 현재 설치 한정 처분 근거는 검수했으나, 전체 Renderer7 P1·다른 플랫폼/플러그인/동적 반사 경로·전체 의존·회귀·클린 복제·게시 이식성은 미수용이다. `RendererGuid7P1Closed`, `DependencyClosure`, `PublicationApproved`, `WholeAccepted`, `LunaReviewed`는 원장상 false로 유지된다. Unity·시험·네트워크·Git 실행과 기존 파일 수정은 없었다.
