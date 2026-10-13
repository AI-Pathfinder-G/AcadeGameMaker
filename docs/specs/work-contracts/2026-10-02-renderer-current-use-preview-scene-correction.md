# 렌더 진단 미리보기 장면 보정

상태: **Approved**, 2026-10-02 통합 책임자 승인. `REQ-PLAT-065..067`, `AC-PLAT-063..065`를 유지한다. v2는 저장하지 않은 초기 장면 때문에 추가 장면 생성이 거절되었다. 실패 결과와 v1/v2 소스는 보존한다.

설치된 Unity 6000.6.0f1 문서의 `EditorSceneManager.NewPreviewScene`, `ClosePreviewScene`, `Camera.scene`과 URP의 카메라 장면 참조를 확인했다. 테라 `gpt-6-sol`은 v2를 새 `PublicationRendererCurrentUseProbeV3.cs`로 복사하여 이름과 출력 접미사를 v3로 바꾸고, 생성은 `EditorSceneManager.NewPreviewScene()`, 해제는 `EditorSceneManager.ClosePreviewScene(scene)`으로 교체하며 카메라 생성 직후 `camera.scene = scene`을 지정한다. 기존 활성 장면 복원과 나머지 관측 논리는 유지한다. 기존 장면 저장이나 교체는 허용하지 않는다.

허용 출력은 새 소스와 메타, `artifacts/c4-renderer-current-use-probe-source-v3.cs.txt`, `artifacts/c4-renderer-current-use-diagnostic-v3` 접두사의 JSON/MD/로그, `docs/verification/2026-10-02-renderer-current-use-v3-terra-report.md`이다. 기존 계약의 보호 집합과 패키지 지문을 실행 전후 확인한다. 정확한 변경을 기록하고 Unity를 한 번 실행한다. 종료 코드와 실제 관측을 구분하며 실패도 보존한다. 이 보정 안에서 추가 재실행은 하지 않는다. 독립 검수 전에는 기존 결함 종결이나 게시 승인으로 해석하지 않는다.
