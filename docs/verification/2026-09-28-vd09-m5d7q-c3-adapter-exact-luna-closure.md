# C3 실행 관리자 정확 소스 검수 종결 — Luna

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 검수.

## 대상과 지문

대상은 `Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs`의
`GetAuthenticatedLaunchRootForNewGame(InputRouter)` 하나다.

- 변경 전 SHA-256: `0DC2D05B209CB49EA6A44DACA0B76687A30597EE999ACE68AD20C6DF93B51D20`
- 보강 후 현재 SHA-256: `1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB`
- Q0 지문 파일 SHA-256: `12B98E06919CDC2AFFBD9E380BF2B4546C00D3BA86A8F86E5D9EDF59FFE331FD`

이 기록은 [이전 P1 검수](./2026-09-28-vd09-m5d7q-c3-adapter-exact-luna-review.md)의
반례를 보강한 후속 기록이며, 이전 기록을 수정하지 않는다.

## 독립 재검수

getter는 이제 `HasCohortDiagnosticMismatch(witness)`를 `HasExactResetLaunchCohort`
전에 호출한다. 이 검사는 adapter/router 양쪽 CWT가 같은 witness인지 확인한
뒤 current receipt와 launch receipt가 witness receipt와 같은지, 그리고
router의 현재 action 묶음이 원본 cohort action과 같은지를 검사한다. 이어서
기존의 Published·Launch 단계·lifecycle·terminal·fault·활성 상태·두 root
witness 검사를 통과해야 하며, 마지막에는 정규화된 witness root만 반환한다.
따라서 이전에 식별한 유효한 외부 receipt/action 반사 주입 반례는 fail-closed
경로로 닫힌다. 함수 본문에는 환경 경로 재조회, 파일 쓰기, lease·profile
observation·receipt·session mutation, C2 호출, 장면·입력 활성화 권한이 없다.

- **AC-M5D7QC3-001:** 정적 소스 기준 PASS, 이전 P1 반례 종결.
- **AC-M5D7QC3-009:** 정적 소스 기준 PASS.
- **AC-M5D7QC3-010:** Unity 회귀를 아직 실행하지 않았으므로 미검증. Terra가
  reflected receipt/action 반례와 집중 시험을 추가할 예정이며, 이를 현재
  통과로 소급하지 않는다.

## Q0 successor 게이트

Q0 파일과 역사적 9행·기존 현재 7행은 변경되지 않았다. 새 adapter 지문은
Q0 successor 행에 아직 반영하지 않았으며, 이 기록은 Astra의 별도 정확 지문
승인이나 Q0 시험 수정 권한을 제공하지 않는다.

## 판정

정적 독립 검수 **P0=0/P1=0**. AC-M5D7QC3-001/009는 소스 범위에서 PASS,
AC-M5D7QC3-010은 Unity 실행 전 미검증이다. Astra가 이 정확 SHA와 후속
시험 범위를 별도로 승인하기 전에는 Q0 지문을 변경하지 않는다.
