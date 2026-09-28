# C3 실행 관리자 후속 소스 지문 승인

날짜: 2026-09-28. 승인자: 아스트라(`gpt-6-astra`). 구현: 테라(`gpt-5.6-terra`). 독립 소스 검수: 루나(`gpt-5.6-luna`).

사용자의 내부 읽기 전용 조회 함수 추가 승인과 Approved C3의 조건부 Q0 후속 증거 계약에 따라 다음 정확한 소스를 승인한다.

- 파일: `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`
- 승인 SHA-256: `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`
- 이전 C2/C2R SHA-256: `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`
- 독립 검수: `docs/verification/2026-09-28-vd09-m5d7q-c3-adapter-exact-luna-closure.md`, 정적 P0=0/P1=0.

첫 후속 소스의 영수증·입력 묶음 일치 누락 P1과 최초 검수 기록은 보존한다. 현재 승인 소스는 조회 함수 내부에서 원래 양쪽 발급 증거·영수증·입력 묶음·라우터·두 경로 증거와 활성 상태를 검사한다. 원래 메서드 본문, 파일 쓰기·초기화·장면·입력 활성화 권한은 변경하지 않는다.

테라는 `HubUiOnlyQ0ScopeAuditEditModeTests.cs`의 현재 실행 관리자 주장 한 개만 위 정확한 C3 후속 SHA와 계약·검수·승인 증거로 이어갈 수 있다. 기존 역사 9행, 엄격한 기존 현재 7행과 C2/C2R 이전 지문을 역사 증거로 보존한다. 입력 관리자 현재 지문 `66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB`은 변경하지 않는다. 이전/새 지문 중 하나를 허용하거나 현재 지문 검사를 느슨하게 하지 않는다.

추적: REQ-M5D7QC3-001/007, AC-M5D7QC3-001/009/010. 본 승인은 정확한 소스·증거 유지 범위의 승인이다. 실제 집중·영향 회귀 전이므로 AC-M5D7QC3-010, C3/C3L 최종 수용 또는 C4 구현 승인을 뜻하지 않는다.
