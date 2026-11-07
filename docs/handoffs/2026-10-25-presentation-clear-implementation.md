# 화면 지우기 보정 구현 및 라이선스 차단 — 2026-10-25

상태: 승인 범위의 보정 후보 구현·정적 컴파일·Luna 코드 검수 완료. 실제 동작 수용은 보류.
권위: Approved docs/specs/work-contracts/2026-10-25-chapter1-presentation-clear.md, ADR-0044/0045. Sol 구현/실행, Luna 독립 검수, Astra 통합 판정. REQ/AC-AUTOCH1-PRESENTATION-CLEAR-001 및 REQ-AUTOCH1-002/003/005/006/008/009, AC-AUTOCH1-002/003/005/006.

## 무엇을 변경했는가

단일 제품 `output/chapter1-playable-r1/project`의 Chapter1ExpeditionPresenter.cs 한 파일에 표시용 카메라 수명을 추가했다. UI만 남은 상태에도 이전 프레임을 지우도록 cullingMask0/depth-1000의 비태그 카메라가 먼저 검정 color와 depth를 clear하고 기존 depth0 world 카메라가 이어 그린다. OnEnable은 자기 child를 생성하거나 재사용하고, OnDisable은 비활성화하며 부모와 함께 소멸한다. 입력·저장·적·피해·밸런스·프리팹·씬은 변경하지 않았다.

기존 UI에는 world 카메라가 없고 방 카메라는 graph 퇴역 때 제거되는 구조가 확인됐다. Presenter는 상태 문자열을 누적하지 않는다. 이 구조는 이전 HUD 픽셀 잔상을 설명하는 후보지만, 실제 출력과 URP 순서를 관측하지 못했으므로 원인/해결을 확정하지 않는다.

현재 핀은 output/chapter1-presentation-clear-20261025/product-source630.v3.json이다. v2 대비 정확1파일 변경, 추가/삭제0. 원문은 before/Chapter1ExpeditionPresenter.cs에 보존했다. v2·v48 및 옛 배포본의 과거 검증은 그대로 유지한다.

## 실제 검사 결과

증거 루트는 output/chapter1-presentation-clear-20261025다.

| 검사 | 실제 결과 | 해석 |
|---|---|---|
| Unity tests 최초 | PID34412,0.3528207초,forced=true/exit=-1 | wrapper CIM 조회가 거부돼 컴파일·시험 전 종료 |
| 실행 도구 보정 | PowerShell 공개 Process.Parent를 자체 생성 pwsh 자식으로 확인, 실제 부모/자식 PID일치 | CIM 경로만 대체, 핸들·출생·exe 신원·정리 유지, 권한 변경0 |
| Unity tests-corrected | PID28180,72.6514488초,forced=false/exit=-536870911 | LicensingClient 초기화60초 timeout, XML없음·시험 통과0 |
| 별도 C# 모듈 컴파일 |11소스/캐시참조271,exit0,8.6256328초 | Unity 번들 csc로 정적 모듈 컴파일만 통과. Unity import/PlayMode/릴리스가 아님 |
| 새 릴리스·실제창 | 실행0 | 시험 통과 선행이 충족되지 않아 빌드/창을 시작하지 않음 |

라이선스 원시의 구체 오류는 `Error: Timed-out after 60.00s, waiting for Licensing to initialize`다. 기존 LicensingClient 채널 연결 거부 뒤 새 client를 기동했지만 초기화되지 않았다. 이번 결과만으로 H: 이동이나 사용자 계정 문제를 원인으로 확정하지 않는다. 로그 침묵만으로 중단한 것이 아니라 명시 오류와 실제 실패 종료를 대조했다. 인증·권한창 대리 처리, 라이선스 파일·키 접근, 관리자 실행, 라이선스 재발급/반환은0이다.

첫 CIM 실패와 두 번째 라이선스 실패는 서로 다른 실패다. 원시 phase별 plan/started/runner-used/closure와 두 번째 player.log를 보존했다. Source 검사 도구는 단독 run-owned.ps1이며 source-correction.json은 실행 도구 소스의 변경을 기록한다. 제품을 다시 바꾼 것으로 표시하지 않는다. Desktop Commander는 등록 장치 offline 결과를 반환해 재접속/설치/우회 실행하지 않았다.

별도 컴파일 자료는 static-compile/{compile.rsp,compiler.log,result.json,AcadeGameMaker.Hub.Presentation.Unity.dll,AcadeGameMaker.Hub.Presentation.Unity.ref.dll}에 있다. 기존 Bee response의 out/refout만 별도 QA 경로로 바꿨으며 참조는 기존 캐시다. 전체 현재 제품의 새 Unity 컴파일이나 배포 바이트 수용으로 확대하지 않는다.

## 독립 검수와 보존

Luna gpt-6-luna가 before/current 코드, 구현 지문,630 v2→v3 및 현재630파일 크기/hash를 대조했고 P0/P1 정적 코드 결함을 발견하지 못했다. 실제 URP 순서·UI 잔상·반복 수명과 월드 가림 여부는 미검증으로 남겼다. 최종 리뷰는 같은 증거 루트의 luna-review.md다.

후검 source630.v3 불일치0·추가파일0, 두 Unity 부모와 추적 자식12개 모두 PID부재다. UPM35048/conhost8260은 소유 신원 확인 후 강제 회수했고 다른 추적 자식은 자연 종료했다. 원본 Ship SHA A4C71B1BFC3E42D02F2F129FE04ADEF3EBF2343A662884E4B99598E9DFC90E7D 및 ZIP SHA82ACE4A9BACFF1385E2951CC0275C9854EF0FD54DDFFC858528B7C5F97ABFB8E는 불변이다.

## 재개 지점

현재 소스는 승인된 보정 후보이며 옛 배포본에 이 코드가 들어있다고 표시하지 않는다. AC-AUTOCH1-PRESENTATION-CLEAR-001, actualWindowAcceptance, finalAcceptance는 모두 false다. Unity 최초시험과 정정시험 예산은 소비됐다. 빌드 슬롯은 미소비지만 시험 통과 선행 때문에 실행할 수 없다. 새 시험을 무변경 반복하거나 이 슬롯을 시험 재실행으로 바꾸지 않는다.

다음 실행 경로는 라이선스 초기화가 가능한 정상 Unity 실행 문맥인지부터 분리해야 한다. 지원된 Unity Hub의 일반 프로젝트 실행 경로가 후보이나 아직 실행·검증하지 않았고, 현재 contract가 GUI 테스트/브리지/새 실행을 승인한 것은 아니다. 일반 프로젝트 실행과 인증·라이선스 변경은 구분한다. 이를 검증할 정확 도구·신원·유한 예산을 묶어 경계 승인 후 진행해야 한다. 로그인/라이선스 활성화나 반환을 자동 대리하는 대안은 사용하지 않는다.

공식 확인 자료: https://docs.unity3d.com/6000.6/Documentation/Manual/EditorCommandLineArguments.html 및 https://docs.unity3d.com/6000.1/Documentation/Manual/ManagingYourUnityLicense.html . 뒤 문서는 CLI 활성화가 Unity Personal에는 적용되지 않으며 Hub 로그인을 사용한다고 구분한다. 현재 계정의 라이선스 종류는 조사하지 않았고 Pro 시리얼이나 Personal 재활성화가 필요하다고 단정하지 않는다.
## Astra 최종 한정 판정
2026-10-25 Astra gpt-6-astra는 Sol 실행 요약과 Luna 독립 검수를 근거로 표시 보정 후보·정적 검사·원문/지문 보존 및 회수 증거만 수용했다. 두 시험 실행 예산은 소진이며 C# 정적 컴파일은 Unity import/PlayMode/릴리스 검증을 대체하지 않는다. 검사 통과 선행 미충족으로 build0/window0 종료를 수용한다. CLEAR AC·실제 잔상 해결·CH1완주·최종전달은 미수용이다. 이번 묶음은 외부 Licensing 초기화 차단으로 종료하고 남은 빌드 슬롯 전용·소진복원·UnityHub GUI 후속실행은 승인하지 않는다. 인증/권한 대리 금지를 유지한다.