# Windows 실제 창 차단: 실행 런타임 원인분리 결과

Sol gpt-6.1-sol, 기본 추론. 사용자 지시 「이슈 해소방법 확인해서 진행」에 따라 기존 불가 판정을 재검토했다. REQ-AUTOCH1-002/003/005/008과 AC-AUTOCH1-001/002/003의 관측 경계에 연결한다. 제품630 및 기존 원시는 쓰기 폐쇄다.

새 확인: Windows computer-use skill의 실제 진입점은 mcp__node_repl__js의 @oai/sky import이며 최신 registry에 제공된다. 따라서 cua_repl의 Native computer APIs disabled만으로 모든 Windows 지원이 없다고 단정할 수 없다. 과거 assessment는 문서 표면만 확인한 한계가 있으며 이번에는 해당 별도 공식 경로를 실제 진단했다.

원시는 preparation/session-resume-20261024/resolution-observations.v1.json이다. 순수 텍스트 확인은284.5초, 환경 재연결 후 skill import는131.9초, 공식 reset 성공 후 순수 텍스트는64.8초 뒤 외부 중단됐다. 세 호출 모두 코드 실행/출력 결과를 관측하지 못했다. import 및 reset후 호출에 timeout_ms10000과 functions.exec yield_time_ms1000을 명시했지만 이번 응답 대기를 제한하지 못했다. 이를 실행 중인 JS가 정해진 시간을 초과했다고 단정할 수 없으며 요청 전송/스케줄링/응답 단계가 미분리다.

공식 js_reset({})은0.3초에 `js kernel reset`, isError=false로 성공했다. 지원된 kernel/bindings 상태변경 뒤 순수 출력도 응답하지 않았으므로 단순 바인딩 손상만으로 해결되지 않았다. reset 경로가 응답하는 사실이 js 실행 서비스 전체의 건전성을 보증하지 않는다. 추가 js/reset은 반복하지 않는다.

로컬 shell 읽기는1.1초 이내 응답하고 Windows/Node exe 및 computer-use26.1016.62342 실물이 존재한다. sandbox 프로세스 목록3개에서 node/helper가 보이지 않았으나 호스트 서비스가 없다고 단정할 수 없다. plugin.json의 hook은 node_repl.turn_ended를 사용하며 shell에서 우회 실행할 공개 helper 경로는 제공하지 않는다. 서비스 종료/재시작·앱 재설치·사용자 프로필/권한 변경0이다.

## 현재 원인 경계와 해결법

1. 과거 Ship의 `Computer Use app approval timed out`는 실제 보호된 UI 접근 단계 실패다. 이번 순수 JS 응답 부재는 게임/창 API 호출 전이며 서로 다른 차단이다. 현재 먼저 해결해야 할 것은 node_repl JS 실행/응답 경로다. 서비스 또는 전송 경로 문제는 후보이며 확정 원인은 아니다.
2. 이번 세션의 공식 커널 초기화는 이미 실행했으나 복구되지 않았다. 최신 codex_app 도구 목록에는 runtime/environment reconnect·repair API가 없다. load_workspace_dependencies는 문서·스프레드시트용 번들 경로 탐색이며 node_repl 복구 도구가 아니다. check_app_update 역시 버전/업데이트 질의용으로 사용하지 않았다.
3. OpenAI 공식 일반 문제해결 문서는 응답 정체 시 앱 새로고침/재시작, 계속되면 시각과 대화 식별자·오류 자료를 모아 Support에 문의하도록 안내한다. 출처: https://help.openai.com/en/articles/7996703-troubleshooting-chatgpt-error-messages (조회2026-10-24 KST, Stuck on Thinking/Generating/Working 및 endless spinner 절). 이는 node_repl 전용 복구 보증이 아닌 일반 안전 복구 후보다. 보안필터·인증·네트워크 설정 변경은 적용하지 않는다.
4. 클라이언트 재연결/재시작은 현재 허용된 에이전트 도구로 수행할 수 있는 공식 복구 API가 없고 ChatGPT 자체 UI 자동화도 금지된다. 셸로 호스트 프로세스를 종료하지 않는다. 따라서 현 세션 내 추가 안전 자동 복구는 확인되지 않았다. 실행 연결이 외부에서 복구된 뒤에만 순수 출력1회로 실제 응답을 확인하고, 성공 시 skill 초기화와 비보호 접근 상태조사의 정확 경계를 새로 정한다. 기존 앱승인 timeout을 동일 보호 호출로 재시도하지 않는다.

이 조사는 실제 커널 reset을 시도했고 실패 경계를 구체화했지만 이슈를 해소하지 못했다. 게임 입력/보호창 호출·Ship/Unity/빌드0, actualWindowAcceptance=false, finalAcceptance=false를 유지한다. 사용자에게 게임 키·재생·반복승인·경로입력을 요청하지 않았다. 제품 실행 예산도 발급·복원하지 않았다.
