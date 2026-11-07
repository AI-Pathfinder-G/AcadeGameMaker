# 첫 입력 원인분리와 실제 마우스 출정 확인 — 2026-10-25

담당 Sol gpt-6.1-sol, 범위/변경 승인 Astra gpt-6-astra, 독립 검수 Luna gpt-6-luna. REQ-AUTOCH1-002/003/005/008, AC-AUTOCH1-001/002/005/006. 사용자 「진행해」에 따른 첫 출정 원인분리다. 제품 변경·Unity·빌드는0이다.

## 결과

정확한 게임 창에서 타이틀바를 클릭해 활성화하고, 새 화면의 출정 버튼을 마우스로 클릭하자 실제 원정 화면으로 전환됐다. 파란 원정 공간, 흰 플레이어, 빨간 적, 체력5/5와 WASD/Space/Shift/공격/전이/선택기술 안내를 관측했다. 따라서 공개 CUA의 일반 마우스 입력으로 기존 Ship의 생산 출정 경로가 이어진 증거를 얻었다.

이후 기본 D1회 뒤 관측은 원정 실패·보존 저장 완료 및 거점으로 버튼이었다. 플레이어가 이동했다는 차이를 확인하지 못했으며 D가 실패를 유발했다고 단정하지 않는다. 적이 접근한 모습과 사망/저장 전이가 관측 사이에 있어 인과 분리가 불가능하다. Space·재시도는 발행하지 않았다. 고립된 자체 SaveData의 profileRevision2/lastOfferedSeed101/completedBranches빈값은 확인했지만, 이것만으로 키 소비나 완주를 입증하지 않는다.

## 각 실행의 실제 실패와 소비

원시 루트: output/chapter1-first-input-20261025. 승인과 정확 경계 변경은 approved-contract.md, 회차별 입력/관측 요약은 run-N/outcome.v1.json, 실제 PID/종료는 evidence/started.json 및 closure.json에 보존했다.

| 회차 | 실행·관측 | 실제 결과 |
|---|---|---|
| run1 | PID20548, getApp158.7923초, 지정10초 tool timeout으로 차단되지 않음 | 175.8777189초 강제회수, 활성/화면/키0. 게임 무반응 시험이 아님 |
| run2 | 동일207exe 경로 사용, getApp0.7417초 | screenshot 전 Raise가 coordinate input geometry is unavailable로 실패. 45.6723209초 강제회수, 게임키0 |
| run3 | screenshot→명확 게임타이틀바[300,15]click→AX/중립화면 | Return duration250 옵션을 Held key presses are unavailable on Windows로 거부. 키 소비 미관측,47.5586209초 강제회수 |
| run4 | 새 screen 기반 titlebarclick→중립화면→출정[1300,575]click, 기본d1회 | 실제 원정 전이와 후속 실패·저장 완료 UI 관측.66.1645252초 강제회수, 이동·점프 미검증 |

게임 프로세스 누적4회, 회차별180초 상한 안에서 회수했고 전체 실제 경과는335.2731859초다. 각 종료 forced=true/exit=-1/error=null이며 정상 게임 종료라고 표시하지 않는다. 전체 프로세스는 독립 후검에서도 부재다. 당초 두 회차의 마우스 대안은 도구 지연/오류로 취소했고, run3·run4는 Astra가 별도 목적/추가 예산으로 명시 승인했다. 기존 모든 실패·소진은 복원하지 않았다. 모든 실행은 종료·소진으로 보존하며 이 보고서가 새 실행을 승인하지 않는다.

## 도구 지원 확인과 설계 수정

- 동일 실행파일 재사용 이후 결속 지연이 줄었지만 이를 원인 확정이나 권한 부여의 증거로 해석하지 않는다. 플랫폼 권한/캐시는 변경하지 않았다.
- AX root의 Raise가 노출됐어도 screenshot geometry를 얻기 전 호출이 실패했다. fresh screenshot 뒤 명확한 제목 표시줄 좌표 클릭은 실제로 성공했다. 전자는 도구 호출 실패이고 게임 결함 근거가 아니다.
- PressKeyOptions 형식에는 durationMs가 있지만 실제 Windows backend는 held key를 거부했다. Sol의 초기 계획은 실제 Windows 지원 범위를 충분히 확인하지 못한 한계가 있었다. 옵션을 반복하거나 비공개 키 다운/업 API로 우회하지 않았다.
- Enter 기본duration1회 후 같은 UI였던 이전 증거는 유지하며, 이번 거부된250ms 호출은 유효한 Enter 무반응 시험으로 집계하지 않는다. mouse 출정 성공을 keyboard 출정 성공으로 바꾸지 않는다.

## 표시 문제의 읽기 조사

run4 실패 후 새 실패/저장 안내와 이전 체력/조작 HUD가 겹쳐 보였다. 실제이미지는 도구 transcript에만 있고 파일로 따로 보존하지 않았으므로 root 관측을 이미지 독립 검증으로 표시하지 않는다.

읽기 조사에서 Chapter1Playable.unity와 Hub.prefab의 직접 Camera 컴포넌트는0이며, ConfigurePresentation이 ScreenSpaceOverlay Canvas를 구성하고 방 카메라는 방 graph에서 인스턴스화된다. Presenter는 _status.text를 대입하며 문자열을 단순 누적하지 않는다. UI만 남을 때 화면 지우기/카메라 수명 또는 캡처 잔상 문제를 분리할 후보지만 이 정적 사실만으로 원인을 확정하지 않는다. 제품 보정은 수행하지 않았다.

## 보존·독립 검수

copy207.v1.json 전수핀과 source630.v2는 후검 불일치0. 원본 Ship A4C71B1BFC3E42D02F2F129FE04ADEF3EBF2343A662884E4B99598E9DFC90E7D, ZIP82ACE4A9BACFF1385E2951CC0275C9854EF0FD54DDFFC858528B7C5F97ABFB8E 유지. 자체 생성 SaveData만 정확 절대경로·링크부재·소유PID부재 확인 후 각 이전 evidence로 지문 보존 이동했고 삭제하지 않았다. 실사용자 profile/보안/인증/권한 조작0이다.

Luna는 run1–3 묶음과 별도 run4 마우스 묶음을 독립 검수했다. 기록은 루트 luna-review.md 및 run-4/luna-review.md다. run4 검수에서는207파일·630핀 전수 및 PID부재도 독립 대조했다. raw screenshot payload와 ZIP 자체의 독립 재해시 한계는 그대로 남겼다. run1 실행 당시 runner 스냅샷은 별도 보존하지 못했으며 소급 복원하지 않았다. run2의 사용본은 evidence/runner-used.ps1, run3/4는 각 회차 runner-used.ps1이다.

## 남은 작업

마우스 출정 성공을 부분 증거로 추가한다. 정상 이동·점프·초점 유지·실패 후 새 출정·양분기 종결·같은 최종 빌드 완주는 여전히 미검증이다. UI 겹침의 제품 렌더링과 캡처 원인 분리도 필요하다. 다음 설계는 지원되지 않는 Windows held 키에 의존하지 않아야 하며, 실제 입력 소비와 표시 결과를 따로 검증해야 한다. actualWindowAcceptance=false/finalAcceptance=false를 유지한다.
## Astra 통합 판정

2026-10-25 Astra gpt-6-astra는 마우스 생산 경로의 출정 전이 관측을 한정 수용하고4회335.2731859초 진단 묶음을 소진종결했다. 이미지는Root관측이며Luna의payload독립재판독결과로확대하지않는다.207/source630핀과PID폐쇄독립대조수용,원본Ship/ZIP불변은Root후검범위다. D이동성공/실패인과·Space·재시도·CH1완주는미검증이다. HUD겹침원인미확정·기존실패/runner스냅샷한계유지. 게임입력전체·이동·재시도·최종수용false. 후속설계/실행은이판정으로승인하지않는다.