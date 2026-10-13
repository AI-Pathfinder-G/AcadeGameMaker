# Renderer2D 조회 코드의 Unity API 호환성 보정

- 상태: **Approved**, 2026-10-02 아스트라 역할 승인. [최초 진단 계약](2026-10-02-renderer-guid7-loaded-probe.md)의 실행은 진단 코드 CS0619로 종료되어 실제 조회가 미실행이다. 원래 실패 결과와 로그는 보존한다.
- 정확 보정: 격리 실행본 `Assets/Editor/PublicationRendererGuidProbe.cs` 원래 SHA-256 `29481075EE7F103A104EF695CC7EB31D16E5DA25B0BD196DBBF465BE56F91794`의 `SerializedProperty.objectReferenceInstanceIDValue` 읽기만 현재 지원 `objectReferenceEntityIdValue.ToString()`으로 바꾼다. DTO는 정수 instanceID 대신 문자열 `ObjectReferenceEntityId`를 기록한다. 설치 Unity의 `Editor/Data/Managed/UnityEditor.xml` 68319줄과 `UnityEngine/UnityEngine.CoreModule.xml`의 EntityId 문자열 변환이 현재 API 근거다. 존재하지 않는 속성의 식별자는 미관측으로 둔다.

원래 소스를 `artifacts/c4-renderer-guid7-loaded-probe-v1-source.cs.txt`에 정확 바이트로 보존한다. 최종 수정 소스 바이트도 `artifacts/c4-renderer-guid7-loaded-probe-v2-source.cs.txt`에 보존한다. 출력 경로만 v2로 변경하고 기타 조회·무변경 경계를 유지한다. 제품 505개·원본·정규화 후보·기존 v1 JSON/MD/로그/보고를 바꾸지 않는다.

정적 차이를 확인한 뒤 수정된 진단을 같은 격리 실행본에서 한 번만 실행한다. 권한 심사·10분 시간 초과·서비스 비종료·실패 시 재실행 금지 등 최초 계약의 조건을 유지한다. 허용 새 결과는 `artifacts/c4-renderer-guid7-loaded-probe-v2.json`, 같은 이름 `.md`·`.log`, `docs/verification/2026-10-02-renderer-guid7-loaded-probe-v2-terra-report.md` 및 위 두 소스 보존 파일이다. 원문 일곱 GUID/fileID/줄과 새 실제 관측을 연결하고 v1의 미관측을 덮어쓰지 않는다.

`REQ-PLAT-047`: 진단 구현의 제거된 API 한 곳을 현재 읽기 API로 보정한다. `AC-PLAT-045`: 원래 소스 SHA 보존, 정적 차이는 위 API/DTO/출력 경로만, 수정 실행 한 번의 실제 결과와 오류를 기록한다. `AC-PLAT-042..044`의 원본/후보 보존·미관측·미수용 조건도 유지한다. 새 실패 발생 시 추가 실행하지 않는다. 루나 독립 검수는 한도 회복 뒤 별도다.

테라 역할 `gpt-6-sol`이 보정과 자체 결과를 작성하고 아스트라가 차이/결과를 기계 대조한다.
