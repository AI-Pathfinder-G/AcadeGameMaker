# C4 선택·기대 행 자료 정적 검토

- Edit 선택 v4 SHA-256: `FCA708318949FB6E1D02B50E142F04CE4F1BD5059C3E0D31E1A048748ABD6B47`.
- Hub 선택 v2 SHA-256: `2D1F07D188F0CC9E802442FF644667707FC6C930165663370D74C5738EA8BFF7`.
- 필수 행 v4 SHA-256: `536BCDB146B5D601C2570A33DC01FD31C41D317D949391BAADF17A29624BC6CD`.
- 큐 도구 SHA-256: `785138F337116614BAA56EB8E250CBC9936855352B91EF6EA422E2E5FE9445ED`.
- 승인 계약 SHA-256: `01D638189329EEE39C52FA663D1EF18242EB321E60875156F7204DCA1FD45791`.
- 판정: P0 0건, P1 0건. Unity 실행은 하지 않았다.

Edit 선택 v4는 v3와 바이트가 동일하다. 139개 기대 이름과 순서가 유지되고 parser 증거는 선언 139/파싱 139, 미지원 선언 0이다. 선택의 단일 `SourceFiles` 경로는 `ProfileResetExecutionBridgeV1Tests.cs`이며 현재 실제 SHA와 일치한다. fixture 변경은 이 선택의 SourceFiles에 들어 있지 않아 선택을 동일 내용으로 재발급한 범위와 맞는다.

Hub 선택 v2는 v1과 비교해 Strict 감사 시험의 `SourceFiles` SHA만 현재 SHA `6EEFEA85B5E1A63BC47B6CF2B8341BAC71B2455BBAD9BEA462DB22679DFCC713`로 바뀌었다. 5개 전체 이름·순서·정규식 선택자·기대 개수는 불변이며 현재 Strict 소스의 다섯 선언과 대응한다. parser 증거는 선언/파싱 5/5, 미지원 선언 0이다.

필수 행 v4는 v3와 비교해 `SourceFiles` 세 경로의 SHA만 갱신됐다. 현재 Edit fixture, Play fixture, Strict 시험의 세 실제 SHA와 일치한다. 나머지 자료는 동일하며 `Rows` 188개와 순서, `EnumSources` 3개가 보존됐다. 세 fixture의 내장 행도 148+35+5개로 합쳐 188개이며 v4 원장의 행 내용과 일치한다.

큐 도구는 결과 경로를 고정된 `artifacts/c4-final-validation-queue-result-v6.json`으로 제한하고 기존 경로 존재 시 실패하는 구조를 유지한다. 해당 v6 결과 파일은 아직 없다. 따라서 실행 자료를 덮어쓰지 않는 사전 상태다. 이 검토는 큐 계획 v7이나 source/input manifest v9의 최종 결속, Unity 컴파일·실행, AC 결과를 수용하지 않는다. 다음 계획/원장 자료는 별도로 정확 SHA와 출력 부재를 검토해야 한다.
