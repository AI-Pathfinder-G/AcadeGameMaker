# Renderer2D 역사 일곱 참조의 현재 범위 테라 원장

승인된 REQ-PLAT-077/078·AC-PLAT-075/076에 따라 Unity 6000.6.0f1의 URP/Core 17.6.0 설치 원문과 현재 격리 Sprite-Lit·점 조명 결과를 읽기 전용으로 대조했다. 역사 일곱 GUID의 자산 대상은 여전히 미해석이다. 로드 Renderer2DData에서 역사 APV 여섯 ObjectReference는 null이고 m_FallOffLookup 속성은 없었다. 역사 여섯 필드는 상속 ScriptableRendererData의 Deprecated 선언에 남지만, 설치 URP/Core C# 직접 참조 검색에서 probeVolumeResources는 그 선언 한 곳뿐이다. m_FallOffLookup의 현 C# 선언·직접 참조는 검색 범위에서 없었다. 범용 ResourceReloader의 반사 재귀와 직렬화·마이그레이션 가능성은 직접 참조 부재만으로 배제하지 않는다.

현재 경로는 별도 Renderer2DResources, 생성되는 Light2DLookupTexture와 useLights 조건부 GlobalPropertiesPass 결속으로 이어진다. Core의 새 ProbeVolumeDebugResources 여섯 객체는 v3에서 로드 확인됐지만 실제 APV 디버그 실행은 관측하지 않았다. v6의 정상 갱신 뒤 켜짐·꺼짐 16픽셀 차이는 현재 격리 점 조명 효과로 한정되며 내부 lookup 샘플의 직접 계측은 아니다.

정확 일곱 원문 행, 소스 줄·SHA, 패키지 C# 검색 범위 지문 및 v2/v3/v6 증거를 JSON에 기록했다. 다른 플랫폼·플러그인·반사·동적 경로, 클린 복제/게시 이식성은 미입증이다. RendererGuid7P1Closed, DependencyClosure, PublicationApproved, WholeAccepted, LunaReviewed는 모두 false이다. 독립 검수와 통합 판정 전 자체 수용하지 않는다.
