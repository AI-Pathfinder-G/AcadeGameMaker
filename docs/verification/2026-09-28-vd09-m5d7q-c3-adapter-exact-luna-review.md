# C3 실행 관리자 원본 경계 — Luna 독립 소스 검수

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 검수.

## 범위

검수 대상은 `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`
의 `GetAuthenticatedLaunchRootForNewGame(InputRouter)` 한 함수다.

- 변경 전 지문: `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`
- 현재 지문: `4CC23B59520FB5C4CF53111758FC6B0A4D7F622B39EABC1CA8FBA2CFE3473DA3`
- 현재 Q0 지문 파일: `12B98E06919CDC2AFFBD9E380BF2B4546C00D3BA86A8F86E5D9EDF59FFE331FD`

## AC별 검수

- **AC-M5D7QC3-001 — P1 결함:** 함수는 null router, adapter/router 양쪽
  참조, 두 `ConditionalWeakTable`의 동일 witness, witness owner/router/
  actions/root, Published 상태, Launch 단계, terminal/lifecycle, router
  fault 및 양쪽 `isActiveAndEnabled`를 확인한다. 그러나 호출하는
  `HasExactResetLaunchCohort`가 `_currentReceipt`와
  `_launchReceiptWitness`를 `Validate()`만 하고 `witness.Receipt`와의
  동일성을 비교하지 않으며, 원본 `witness.Actions`와 현재 router action
  묶음도 대조하지 않는다. 반사로 유효한 외부 receipt 또는 action 상태를
  주입하면 root witness가 정상이어도 함수가 원래 cohort root를 반환할 수
  있어 AC의 foreign/reflection-corrupt fail-closed 행을 위반한다. Terra가
  이 검사를 보완하고 새 지문을 제출하기 전까지 P1=1이다.
- **AC-M5D7QC3-009 — 정적 PASS:** 함수에는 환경 경로 조회, 파일 쓰기,
  lease·profile observation·receipt·session 상태 변경, C2 호출, 장면·입력
  활성화·map·wardrobe·network·clock·random 권한이 없다. 반환값은 기존
  launch cohort root뿐이다.
- **AC-M5D7QC3-010 — 미검증:** Unity 집중·필수 회귀를 실행하지 않았다.
  따라서 `0` 실패·건너뜀·판정 불가 또는 최종 수용을 주장하지 않는다.

## Q0 후속 지문 게이트

Q0 테스트의 현재 successor 행은 아직 실행 관리자 이전 지문
`0DC2D05...51D20`을 고정하고 있다. 새 지문 `4CC23B...7DA3`으로 바꾸지
않았으며, 이는 계약이 요구한 Luna의 정확 소스 검수와 Astra의 별도 지문
승인 전에는 올바른 상태다. 역사적 9행과 기존 현재 7행을 대체하거나
구 지문·신 지문 중 하나를 허용하는 우회는 발견하지 못했다.

## 판정

정적 소스 검수 P0=0/P1=1. AC-001은 위 P1 결함으로 보류하고 AC-009는
소스 범위에서 PASS, AC-010은 Unity 실행 전 미검증이다. 이 기록만으로 Q0
테스트를 수정하거나 지문을 승인하지 않으며, Terra 수정과 재검수 및
Astra의 별도 successor 지문 승인 후에만 다음 단계가 가능하다.
