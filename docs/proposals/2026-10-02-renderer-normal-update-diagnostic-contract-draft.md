# Renderer2D 정상 조명 갱신 이후 렌더 진단 계약 초안

상태: **Draft**. 이 문서는 v5 실패 원인을 확정하지 않으며 코드 변경·Unity 실행 승인도 아니다. 제안 식별자 `REQ-PLAT-075/076`, `AC-PLAT-073/074`는 작성 시 문서 검색에서 미사용이었다. Renderer 일곱 GUID P1, 전체 의존 폐쇄와 게시 허가는 유지한다.

## 실제 경계와 미확정 원인

동결 v5 소스 `output/u505-r1/Assets/Editor/PublicationRendererLightingGateProbeV5.cs` SHA-256 `A2A6B8F6DA2D1FA54FFE22A4E1EDBE9A295DB9A6BBDB6F0BFDC3D65E74D9E5ED`의 `Run`은 `ProbeRender`를 동기 호출한다(335–356행). `ProbeRender`는 조명 구성 직후 같은 호출에서 점 조명 켜짐 렌더→`point.enabled=false`→꺼짐 렌더를 수행한다(295–317행). 실제 명령에는 `-batchmode -quit -executeMethod PublicationRendererLightingGateProbeV5.Run`이 있었다(`artifacts/c4-renderer-lighting-gate-diagnostic-v5.log:14–22`). v5 결과는 두 제출·같은 RGBA SHA·차이 픽셀 0이다(`docs/verification/2026-10-02-renderer-lighting-gate-v5-terra-report.md`). 이 사실은 정상 갱신 누락과 양립하지만 원인 확정은 아니다.

설치 URP `Light2D.cs` SHA-256 `183EE4B7FBDC4ADE89A25779D862418B9EF0C142328DB9F1BECB6FA45999B113`는 `[ExecuteAlways]`(14행), `OnEnable`의 등록(651–656행), `LateUpdate`의 `UpdateMesh(forceUpdate)`·`UpdateBoundingSphere`(674–683행)를 분리한다. `UpdateMesh`는 점 조명 메쉬를 구성하고(541–583행), `UpdateBoundingSphere`는 점 조명 위치·반경을 기록한다(601–607행). `CacheValues`(470–473행)는 캐시 위치 기록이며, `Renderer2DRendergraph.cs` 900행에서 컬링된 조명에 호출된다. 따라서 `CacheValues`만 관측해도 앞선 `LateUpdate`·컬링 성공을 증명하지 못한다. `Light2DCullResult.cs` 133–165행은 실제 카메라에 보이는 조명 목록과 `boundingSphere`로 점 조명을 거른다.

설치 Unity `6000.6.0f1`의 `Editor/Data/Managed/UnityEditor.xml` SHA-256 `A7479F4EFD196F1A709B3370B3D0B217FABEAE3C2A145F03FE97F6DA448E6AD8`는 `EditorApplication.update`가 주기적으로 호출됨(15986행), `QueuePlayerLoopUpdate`가 수정 여부와 무관하게 플레이어 루프 갱신을 예약함(16138행), `EditorApplication.Exit(int)`이 편집기를 종료함(16029행)을 기록한다. 이는 갱신을 **요청**할 수 있다는 근거이지, 같은 콜백에서 이미 `LateUpdate`가 끝났다는 보장은 아니다.

## 단계형 v6 진단 후보

`REQ-PLAT-075`: v5 원문·결과는 불변으로 보존한다. 별도 Approved 허용 목록의 v6 편집기 진단은 기존 격리 `output/u505-r1`에서만 실행한다. `-batchmode -executeMethod <새 v6 Run> -logFile <새 로그>`로 시작하되 `-quit`은 **빼고**, `Run`에서는 장면·카메라·Sprite-Lit 재질·두 정상 `Light2D` 구성과 콜백/기한 등록까지만 한다. 두 조명 생성 직후 `EditorApplication.QueuePlayerLoopUpdate()`를 호출하고 `Run`을 반환한다. `EditorApplication.update`의 다음 틱에서 곧바로 렌더하지 않고, 서로 다른 틱에서 최소 두 차례 루프 갱신을 요청·관측한다. 시간 경계는 구성 완료 뒤 45초, 외부 프로세스 전체 180초로 제한한다. 틱 횟수만으로 완료를 주장하지 않는다.

`AC-PLAT-073`: 구성 직후와 갱신 뒤 점 조명의 내부 `boundingSphere`를 **읽기 전용**으로 관측해 중심이 실제 Transform 위치, 반경이 설정한 양수 외곽 반경과 일치하는지 기록한다. 조명·카메라가 같은 미리보기 장면, 등록 목록에 동일 참조, 활성화, 계층·컬링 마스크도 재확인한다. 읽기용 정확 속성이 없거나 업데이트 뒤에도 구가 불일치하면 렌더 대조를 수행하지 않고 `UpdateUnverified`로 종료한다. `LateUpdate`, `UpdateMesh`, `UpdateBoundingSphere`, `CacheValues`를 반사로 직접 호출하거나 private 필드를 쓰는 방식은 금지한다. 이 관측은 정상 결과와 일치함을 보여줄 뿐 `LateUpdate` 호출 계수 자체는 아니다.

`REQ-PLAT-076` / `AC-PLAT-074`: 정상 갱신 조건이 확인된 뒤에만 **한 Unity 프로세스에서** 점 조명 켜짐과 꺼짐 실제 `SingleCameraRequest`를 각 한 번 제출한다. 켜짐 프레임 전 정상 틱을 거쳤다는 사실, 두 프레임의 카메라·재질·Sprite·전역 조명 동일성, 점 조명 활성 상태, 각 카메라 시작/종료 콜백, RGBA 원시 SHA·차이 픽셀 수를 기록한다. 꺼짐 전환 뒤 다시 정상 루프 갱신을 거친 다음 둘째 프레임을 제출한다. 프레임 사이에 점 조명 `enabled` 변경 외 fixture 조건을 바꾸지 않는다. `UnityBlack`을 종료 후 읽은 값은 내부 패스 증거로 쓰지 않는다. 차이가 없으면 갱신 이후에도 조명 영향 미입증으로 남긴다. 차이가 있어도 `useLights` 내부 값·lookup 소비·Renderer GUID7 P1 폐쇄는 별도 실제 패스 증거 없이는 주장하지 않는다.

v6은 `EditorApplication.update` 구독을 정확 한 번 해제하고 미리보기 장면/임시 객체/렌더 자원을 `finally`에서 정리한 뒤 **새 출력** JSON을 `CreateNew`로 쓰고 `EditorApplication.Exit(0)`을 호출한다. 실패·예외·기한 초과도 같은 정리와 실패 JSON 시도 후 비영(非零) 종료한다. 외부 감시가 전체 180초를 넘으면 프로세스 중단·실패를 기록하고 성공 산출물로 승격하지 않는다. `-quit`을 같이 사용하면 `Run` 반환 뒤 예약된 갱신/지연 콜백 전에 편집기가 종료될 수 있으므로 이 방식에서는 금지한다. 출력·로그 및 보호 입력의 사전/사후 SHA, 실제 실행 종료값, 중복 콜백/출력 거절, 루나 독립 검수는 별도 Approved 구현·검증 계약에서 정확 경로로 동결한다.
