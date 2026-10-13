# Renderer2D 현재 사용 경로 진단

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 별도 테라 `gpt-6-sol`이 격리 실행본에만 진단을 구현·실행한다. [설계 초안](../../proposals/2026-10-02-renderer2d-current-use-diagnostic-contract-draft.md) SHA-256 `F23C3C59C0E485E4CB531C1A86425374B315CD5A650E8E87E3A9D891AABD74B8`의 정확 입력·실행 순서·출력·보존·관측 한계를 아래 보충 조건과 함께 채택한다. 원래 Draft와 기존 GUID7 P1은 보존한다.

`REQ-PLAT-065`/`AC-PLAT-063`: 현재 전역 설정의 APV 디버그 자원 여섯 개를 정확 관리참조형식과 실제 로드 객체·GUID·설치 패키지 메타에 결속한다. 조건부 APV 디버그의 실제 소비는 이번 실행에서 호출하지 않고 미관측으로 구별한다.

`REQ-PLAT-066`/`AC-PLAT-064`: 현재 유효 UniversalRP 및 진단 카메라의 실제 Renderer2D 선택, 저장하지 않는 임시 2D조명 장면의 지원 공개 render request 한 번, 해당카메라 실제 콜백·출력 픽셀·falloff 전역 텍스처 관측 및 오류를 기록한다. GPU/지원 API/콜백/전역 텍스처를 관측하지 못하면 미관측 또는 실패이며 성공으로 승격하지 않는다.

`REQ-PLAT-067`/`AC-PLAT-065`: 원문534·정규화534·정규화505·실행본505와 정확 설치패키지/API의 전후SHA가 동일하고, 새 진단 source/meta·실제 단일Unity 실행·버전/그래픽장치/UTC/종료/로그 지문을 보존한다. 기존 원문/후보/소스/자산/설정/패키지/증거/Git/원격을 수정하지 않는다.

보충: 설치 API의 공개 지원 표면을 확인하고 컴파일 제거/오류형식 참조를 피한다. `GraphicsSettings.defaultRenderPipeline`을 우선 사용하고 제거·obsolete-error API로 우회하지 않는다. rendererDataList 접근이 공개가 아니면 SerializedObject로 해당필드·기본인덱스를 정확 읽으며, 실제 카메라 renderer는 공개 getter로 관찰한다. 카메라/Sprite/Light2D/Material/Texture/RenderTexture는 해당 격리 프로젝트에서만 잠깐 생성·해제하고 Scene/Asset 저장·Refresh/ForceReserialize·패키지 변경·기존 장면 저장은 금지한다. 임시 장면은 기존 장면을 수정/저장하지 않도록 additive 새 메모리 장면에 한정한다.

테라가 설치 소스를 대조한 후 새 진단 source를 작성하고 실행 전 SHA/본문·기존 보호집합 전체지문을 기록한다. Unity는 새 결과가 모두 부재하고 해당프로젝트 다른실행이 없을 때만 정확 `6000.6.0f1`로 한 번 실행한다. 새 log/native결과 보존을 위해 시작·종료기록을 JSON에 한 번 결합할 수 있고 결과본문 원시 보존을 요구한다. 출력JSON은 자기해시를 포함하지 않는다. 외부 래퍼가 시각/종료/전후 지문·sourceSHA를 결합할 때도 원시 관측을 보존한다. 오류 후 패치·재실행은 이 계약에서 허용하지 않는다.

허용 출력은 Draft의 다섯파일(진단JSON/MD/log/source본문보존/테라보고) 및 `output/u505-r1/Assets/Editor/PublicationRendererCurrentUseProbe.cs`와 그 Unity생성meta뿐이다. 필요하면 원시 진단 관측을 같은 JSON의 `RawObservation`으로 보존한다. 원문505/534 지문 차이·컴파일/렌더 오류·장치부재는 사실 그대로 기록한다. 독립 루나 결과와 아스트라 기술판정 전에는 P1·게시·전체수용·의존폐쇄를 닫지 않는다. 한도/인증/모델 오류는 반복하지 않는다.
