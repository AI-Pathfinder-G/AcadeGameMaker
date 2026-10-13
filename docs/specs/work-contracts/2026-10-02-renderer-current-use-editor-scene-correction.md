# 렌더 진단 편집기 장면 생성 보정

상태: **Approved**, 2026-10-02 아스트라 역할 승인. 소유 `REQ-PLAT-065..067`, `AC-PLAT-063..065`와 기존 현재사용 진단의 보존/출력/단일실행 한계를 유지한다. v1 실행은 종료0이지만 편집모드 `SceneManager.CreateScene` 예외로 렌더 관측에 도달하지 못했다. 결과 SHA `6E6ED491EA8E6B60FA7C80485FAC1028762FCDB40C47F0C8097FA3A5AFD0DA1E`, source SHA `2252F9964592D3E9FBB6FAA65DA631DF3F8D72BCAECCEAC8501DD344A7ADB7BF`, log SHA `43825B866D9263A8B8712A13051247A27628E11FFBC2C99D5805E6C8B1E61751`는 불변 보존한다.

테라 `gpt-6-sol`은 새 `output/u505-r1/Assets/Editor/PublicationRendererCurrentUseProbeV2.cs`를 v1에서 복사해 클래스/임시장면 이름과 결과출력 stem을v2로 바꾸고 장면 생성 한 곳을 `EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive)`로 교체할 수 있다. 기존 `EditorSceneManager.CloseScene(scene,true)` 해제는 유지한다. 기존 활성장면을 바꾼다면 진단이후 원래 활성장면을 복원하되 기존장면/자산 저장은 하지 않는다. 나머지 APV/파이프라인/렌더/조명/필터/판정 논리는 바꾸지 않는다.

새 결과 stem `artifacts/c4-renderer-current-use-diagnostic-v2`의JSON/MD/log, `artifacts/c4-renderer-current-use-probe-source-v2.cs.txt`, `docs/verification/2026-10-02-renderer-current-use-v2-terra-report.md` 및 새source/meta만 허용한다. v1 모든소스/결과/원문/후보/캐시/Git/원격은 불변이다. source의 정확 변경 diff와 사전SHA/본문을 기록한 뒤 v2 executeMethod로 Unity 한 번만 추가 실행한다. 실행 전후 보호집합·패키지/API지문과 실제 종료/관측/오류를 기록한다. 실패·미관측이면 상태그대로 독립검수에 넘기며 이 계약에서추가패치/반복실행은 금지한다.
