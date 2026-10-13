# 조명 진단 장면 핸들 호환 보정

상태: **Approved**, 2026-10-02 통합 책임자 승인. `REQ-PLAT-073/074`, `AC-PLAT-071/072`와 조명 대조 계약의 관측·보호 한계를 유지한다. v4는 설치 Unity에서 폐기된 SceneHandle→int 암시 변환 다섯 곳의 CS0619로 컴파일 실패했으며 실제 렌더는 미관측이다. JSON SHA `0DA00645A6BC94BE26D225B64A5C098CD42DA03D5B98A6C623114D912C706AF3`, 소스 SHA `3572CB653A74E8C1A3B4D5E8BEF7686156DE6466D25F5FDBA75AB039A873A102`와 로그를 보존한다.

테라는 v4 원본 소스와 메타를 정확 바이트 그대로 `output/renderer-lighting-gate-v4-source-history/`로 옮겨 컴파일 범위 밖에서 보존할 수 있다. 두 실제 절대 경로가 작업공간 안인지 확인하고 개별 파일만 이동한다. 복사와 이동 전후 SHA·기존 경로/보관 경로를 기록한다. 원본 소스 텍스트 보관과 v4 결과는 그대로 둔다. 잘못된 소스를 Assets에 두어 후속 컴파일을 막지 않기 위한 진단 소스 이동만 승인한다.

새 `output/u505-r1/Assets/Editor/PublicationRendererLightingGateProbeV5.cs`와 메타를 구현한다. v4에서 클래스/출력 접미사를 v5로 변경하고 정수 결과에 대입하는 `.handle` 다섯 곳만 설치 API의 `.handle.GetRawData()`로 교체한다. 읽기 전용 등록 검사 등 나머지 로직은 유지한다. 소스 전체 diff를 기록한다. API 문서와 실제 반환 타입을 확인한 뒤 Unity 한 번을 실행한다.

실제 설치 DLL 확인으로 `GetRawData()`가 `System.UInt64`를 반환한다. 따라서 다섯 결과 필드를 함께 선언하는 DTO 한 행도 `int`에서 `ulong`으로 변경해 값 손실 없이 보관하도록 명시적으로 승인한다. 이 타입 변경을 정확 diff에 별도로 기록한다. 부호나 축소 변환으로 우회하지 않는다.

새 허용 출력은 `artifacts/c4-renderer-lighting-gate-probe-source-v5.cs.txt`, `artifacts/c4-renderer-lighting-gate-diagnostic-v5.json`, 같은 접두사 로그, `docs/verification/2026-10-02-renderer-lighting-gate-v5-terra-report.md`와 위 새 소스/메타·v4 보관 두 파일이다. 조명 켜짐/꺼짐 각 한 요청과 원시 픽셀 비교·보호534/505/캐시/설정·이전 결과 불변을 유지한다. 이동으로 인한 경로 변경은 별도 증거로 명시한다. 추가 실패는 보존하고 계약 내 임의 패치·재실행하지 않는다. GUID7 무영향·전체 폐쇄·게시·독립 수용은 미완료다.
