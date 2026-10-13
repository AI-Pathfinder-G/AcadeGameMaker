# Renderer2D 일곱 GUID 출처 평가 독립 검토

2026-09-29. 실제 모델 `gpt-6-luna`. 검토 대상은 Sol 평가 JSON SHA-256 `4D19A77A2475FEBB5E2504F6536932723D1D03EF7AEEA278EE24A387E727A5DC`와 설명 SHA-256 `F67B72345E3735230AA613E67B96EC0F9044FD9C605AE1F0D6EF406B0C6259B0`다. 참조 파일 52개 지문과 인용 줄 범위를 대조한 결과 불일치가 없었다. 또한 Assets 및 현재 PackageCache의 메타 파일을 읽기 전용 검색하여 일곱 저장 GUID의 정확 메타 일치가 0개임을 확인했다.

평가의 핵심 구분은 정확하다. `Renderer2D.asset`의 여섯 `probeVolumeResources` 값은 `Deprecated.cs`에서 Shader 4개, Mesh 1개, Texture2D 1개로 선언된 과거 직렬화 필드다. 대응 ResourcePath 자원은 현재 존재하지만 여섯 자원의 각 meta GUID는 직렬화된 값과 다르다. URP 샘플에 같은 여섯 GUID가 같은 필드로 저장된 사실도 확인되지만, 이것은 샘플 직렬화 패턴이지 현재 GUID 대상의 해석이나 자동 이관 증명이 아니다. 일곱 번째 `m_FallOffLookup`은 정확 필드 타입·대상이 여전히 확인되지 않았다. 관련 Texture2D 생성·렌더 사용 경로를 별도 근거로 두면서도 이를 무영향 증거로 쓰지 않는다. 일곱 항목 모두 `미해석`, `MissingAssetProved=false`, `NoEffectProved=false`, `PublicationApproved=false`로 남아 있다.

버전 근거도 경계가 맞다. 현재 manifest는 URP 17.3.0을 요청하지만 lock과 두 현재 PackageCache는 17.6.0이며 lock 출처는 `builtin`이다. 현재 lock SHA는 최초 후보 바이트와 같고, 이 차이를 새 업그레이드나 결함으로 단정하지 않는다. 9월 8일 M5C2 계약·검증은 당시 실제 URP 17.6.0과 manifest 요청 17.3.0의 차이를 이미 구분했으나 당시 패키지 지문은 현재와 다르고 수용 범위는 카메라 단위다. 이를 일곱 GUID의 현재 수용으로 옮기지 않는다.

역사 실행도 과장하지 않는다. 8월 25일 Bootstrap 자료는 Unity 6000.3.21f1 범위다. 9월 6일 갱신 경과에는 초기 bootstrap 차단 뒤 6000.6.0f1에서 Bootstrap 1/1과 Edit 372/372를 기록한 실제 XML·로그가 있으나, 보충 감사 자체는 별도 Luna 검수가 없었다. 이 증거는 당시 실행·버전 기록이지 현재 GUID 일곱 개의 해석이나 플랫폼 전체 빌드 독립 수용을 뜻하지 않는다. 과거와 현재 package 지문 차이도 명시되어 있다.

**판정: P0 0, 새 결함 P1 0. 기존 Renderer2D GUID 7개 미해석 게시 차단 1묶음은 계속 P1로 열린다.** 원장·자산·설정은 변경하지 않았고 Unity import, 컴파일, Git, 네트워크를 실행하지 않았다. 실제 GUID 대상·타입·fileID 또는 미해석을 판정할 정확 버전의 import 로그, 자산/meta 전후 지문과 승인된 빌드 진단 전에는 누락·무영향·결함·버전 변경·C3 무효 또는 게시 승인을 결론내리지 않는다.
