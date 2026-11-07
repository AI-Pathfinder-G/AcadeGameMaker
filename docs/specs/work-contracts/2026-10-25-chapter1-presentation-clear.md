# CH1 UI 전환의 화면 지우기 보정

- 상태: Approved — 2026-10-25 Astra gpt-6-astra, presentation_bundle_approval 판정.
- 권위: ADR-0044/0045, Approved 2026-10-21-autonomous-chapter1-delivery. Sol 구현·실행, Luna 독립 검수, Astra 통합 판정.
- 사용자 산출물: 이전 화면 잔상이 없는 거점·실패·재시작 표시와 현재 소스 Windows 후보. CH1 완주 수용은 별도다.

**REQ-AUTOCH1-PRESENTATION-CLEAR-001:** 거점 등 world 카메라가 없는 UI 상태에서도 매 프레임 배경 color와 depth를 지워 이전 HUD/월드 픽셀이 누적되지 않아야 한다. 활성 원정의 기존 world 표시와 UI를 가리거나 입력·저장·적·피해·선택 행동을 변경하지 않는다.

**AC-AUTOCH1-PRESENTATION-CLEAR-001:** 같은 최종 후보의 실제 릴리스 창에서 초기 UI, 정상 원정 world, 자연 실패 후 이전 HUD 잔상 없는 실패 UI, 거점 복귀와 재출정을 관측한다. 카메라 설정이나 합성 입력 시험 통과만으로 시각적 해결을 선언하지 않는다. 실제 이미지의 독립 재판독 한계가 있으면 명시한다.

구현은 output/chapter1-playable-r1/project/Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/Chapter1ExpeditionPresenter.cs 정확1파일에 한정한다. 비태그 자기child Camera를 OnEnable에서 한 번 생성하거나 재활성화하고, OnDisable에서 비활성화하며 parent 소멸 때 함께 정리한다. cullingMask0, depth-1000, SolidColor/검정, orthographic, near.01/far1, HDR/MSAA off. 항상 먼저 color+depth를 clear하고 기존 depth0 world 카메라가 그 뒤 렌더링한다. 실제 URP 순서는 관측 전 선취하지 않는다.

source630.v2/v48/oldShip207/ZIP 및 변경 전 원문을 보존하고 현재source630.v3와 새 빌드 전체 지문을 별도 작성한다. 단독 실행 도구는 output/chapter1-presentation-clear-20261025/run-owned.ps1이며 실행 중 제품 쓰기·같은 프로젝트 동시 Unity는0이다.

Unity 예산은 최대3회다. 기존 Chapter1ExpeditionPlayModeTests의 AuthoredHubSubmitStartsExpedition, AuthoredHubClickStartsExpedition, HeldSubmitDoesNotStartOrRestart, AuthoredFailureReturnsAndAllowsFreshStart 정확4개를 처음1회900초 실행한다. 원인 있는 허용1파일 보정 뒤 국소 재실행1회900초가 가능하고 무변경 재시도는0이다. 통과 뒤 기존 Chapter1AutonomousBuild.BuildWindows로 새 attempt-001 릴리스 빌드1회1800초를 수행한다. 기존 출력 덮어쓰기는0이다.

Unity 파생 허용은 신규 InitTestScene*.unity/.meta와 Resources.meta, Resources/PerformanceTestRunInfo.json/.meta, PerformanceTestRunSettings.json/.meta뿐이다. 회차 생성·기존 부재·지문·프로세스 종료 확인 후 증거로 보존 이동할 수 있다. 다른 원인 불명 제품 차이와 라이선스/권한 오류에서는 중단한다. 기존 시험의 자체 expedition-GUID 격리 저장 leaf만 생성·정리하며 실사용자 저장에 접근하지 않는다.

새 빌드 실제 창은 최대2회, 각240초·총480초, 캡처12·click8·기본키2다. 두 번째 실행은 첫 결속 지연으로 시험 미도달일 때만 가능하다. fresh화면·titlebar좌표 활성화·마우스출정·자연실패·거점으로·재출정을 관측한다. HP강제·Windowsheld옵션·Raise·직접OS입력·권한/보안창 처리는0이며 지원/가림/보안 차단 시 회수한다. 기존 소진 예산은 복원하지 않는다.

종료 조건은 source/빌드/실행 증거, Luna의 묶음 후 독립 검수1회, Astra 통합판정이다. 실제 CH1 전체 양분기·키 이동·최종 사용자 전달 수용은 이 계약 승인으로 통과하지 않는다. 기존 REQ-AUTOCH1-002/003/005/006/008/009 및 AC-AUTOCH1-002/003/005/006에 함께 연결한다.