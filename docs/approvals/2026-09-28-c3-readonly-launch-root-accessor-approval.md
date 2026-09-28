# C3 실행 관리자 내부 조회 함수 추가 승인

날짜: 2026-09-28. 승인자: 사용자. 범위 통합: 아스트라.

사용자가 이번 세션의 좁은 승인 요청에 대해 `해당 읽기 전용 함수 추가 승인`을 선택했다. 이전 세션의 미답변 요청을 승인으로 소급하지 않는다.

대상은 `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`의 내부 함수 `GetAuthenticatedLaunchRootForNewGame(InputRouter)` 하나다. 기존 실행 소유자·입력 관리자·두 프로필 경로 증거를 확인해 원래 경로만 반환한다. 변경 전 SHA-256은 `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`이다.

이 승인은 새 환경 경로 조회, 파일 저장·초기화, 입력 활성화, 장면 전환을 허용하지 않는다. Approved C3/C3L 계약의 나머지 경계는 유지한다. Q0 지문 변경은 정확한 후속 실행 관리자 소스에 대한 루나 검수와 아스트라의 별도 SHA-256 승인 이후에만 가능하다.

구현 추적: REQ-M5D7QC3-001/007. 검증 추적: AC-M5D7QC3-001/009/010. 본 문서는 사용자 승인 기록이며 구현·실행·최종 수용 증거가 아니다.
