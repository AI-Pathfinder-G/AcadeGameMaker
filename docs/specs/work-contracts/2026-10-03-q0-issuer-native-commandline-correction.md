# Q0 발급 진행자의 실제 명령행 보정

상태: **Approved** — 2026-10-03 실제 `gpt-6-astra`. `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068`의 실행 입구 결속을 보정한다. 4F9/19DC/E4AA/3084 계약 원문 및 준비570·계획53·원격 정확 두 문서 범위는 유지한다.

## 실제 실패와 원인

첫 승인된 소유 TTY의 issuer37840/전체ticks639265682109360254/session55173은 Boot에서 `IssuerBoundaryRejected`, NewSessionInvocationCount=0, 실제 종료2로 보고됐다. 키 생성 전 실패이며 같은 키 재발급이 아니다. source6516A476EE47881C56712BDC33E779C807F64405E411FF7B300F7AFE2F0B4EEE, test95762DFAE8262660D478A124394DE11372E8128A9408AD0BEE8DC4851E7880A4, v2증거0CEB73D3BAF0704621824123338AE71186F2BF940764CDCE7DCAAC9954580C97 및 당시 handoff1EC50BA368A1EA378B2572EE9181CE82DEDA8E12FA92EFA9C47B6EEBC5C8870F는 역사로 보존한다.

Root/Core 실제 키 없는 관측에서 관리형 GetCommandLineArgs/Environment.CommandLine의 첫 토큰은 pwsh.dll, 실제 MainModule은 pwsh.exe였다. Core가 kernel32.GetCommandLineW 원문을 기존 CommandLineToArgvW 파서에 전달했을 때 실제 exe와 Ordinal 일치했다. 앞선 불완전 AST 진단의 InputSchema 누락/PSScriptRoot 손실은 별도 진단 실패로 남긴다. 이 관측을 Astra 직접 실행으로 소급하지 않는다.

## 허용 변경

기존 `qa/tools/Invoke-Q0TrustedChildIssuer.ps1`의 `Q0IssuerCommandLine` 타입에 kernel32.GetCommandLineW 읽기 메서드를 추가하고, 타입 적재 후·Preflight 전에 현재 프로세스의 네이티브 원문을 기존 Parse로 해석해 entry argv를 고정한다. null/비어 있음/파싱 실패는 키 전 거절한다. actual MainModule 경로와 첫 토큰, 정확5토큰 및 -NoProfile/-File/고정 issuer 절대경로/-TrustedIssuer, 실제 실행 파일 SHA 검사는 유지한다. DLL→EXE 문자 치환·관리형 fallback·검사 삭제·대소문자 완화는 금지한다. 실제 entry path 캡처와 native argv의 동일 프로세스 결속을 유지한다.

기존 `qa/tools/Test-Q0TrustedChildIssuer.ps1`에 해당 실제 API·entry helper의 양성/음성·실제 파일 입구 검수를 추가한다. 운영 mock/진단 CLI 스위치는 추가하지 않는다. 키 없는 별도 시험에서만 효과를 차단한다. 일반 ScriptBlock 재작성 때문에 파일 메타데이터를 잃은 시험을 실제 파일 Preflight라고 부르지 않는다. 실제 issuer 파일 입구에서 네이티브 캡처가 수행되고 Abort/거절로 NewSession0·실제 종료가 관측되는 시험과, 정확 원문 helper에 실제 네이티브 토큰을 주는 양성/위조 토큰 음성을 구분한다. 전체 Preflight 양성을 주장하려면 원래 파일 경로·PSScriptRoot·PSCommandPath·호출 인자 맥락을 보존한 근거가 별도로 필요하다. 모의 승인 줄을 실제 handoff에 발급하지 않는다.

추가로 다른 exe/File 경로/추가 인수/잘못된 sourceSHA를 거절하고, 관리형 dll 토큰은 실제 exe 결속을 대체하지 못함을 확인한다. 범위 보정 뒤 실제 LibraryOnly 연결 및 Bind/Sign 모의 회귀는 영향 범위에서 검수한다. 실제 CNG/New/SignHash/remote0을 유지한다. Q0IssuerInputPump 클래스 원문/API는 변경하지 않는다. 최종 source의 클래스 원문이 이전 수용본과 동일함을 대조하면 별도 Root controller 네 릴레이 행동을 재실행할 필요는 없다. 전체 타입코드가 바뀌었음을 숨기지 않는다.

## 증거와 앵커

새 증거는 정확 `output/q0-trusted-child-evidence-r1/issuer-control-keyless-v3.json` 한 곳만 허용한다. Root가 원시 후보를 검수한 뒤 동일 bytes를 CreateNew 한 번 보존하고 Luna가 실제 파일 전체와 변경 소스를 독립 검수한다. 이전v1/v2·수집 실패·첫 Boot 실패는 불변 보존한다. 실패한 새 증거를 덮거나 임의v4를 만들지 않는다.

새 독립 접두사는 유일 `IssuerControlLunaReviewV3Json: `, 새 시작 접두사는 유일 `IssuerControlStartAcceptanceV3Json: `다. 기존 V1/V2 독립 줄과 기존 시작 줄은 수정·복제하지 않는다. 각각 기존 정확10키·기존 SchemaVersion/AcceptanceId/Decision/ContractSha256=19DC 구조를 유지하며 최종 source2/v3증거 SHA를 사용한다. 새 시작 IndependentReview는 V3 JSON 원문 UTF8 SHA(줄끝 제외)만 참조한다. Boot은 새 접두사와 v3 정확경로만 수용하고 본 계약 원시 SHA도 확인한다. 이전 시작 줄을 대안으로 받아들이지 않는다.

Root 메모리 큐 진행자의 시작 앵커 검색과 시작 전 사전검수도 V3 접두사로만 갱신한다. 그 외 소유/전송/기한/실패반환 본문은 유지하고 변경된 메모리 원문SHA를 다시 기록한다. 전체 handoff SHA는 모든 실제 검수/수용 줄 발급 뒤 Root가 직접 고정한다. 키 없는 독립 검수 후 실제 Astra가 최종 소스/증거/검수 원문을 읽고 V3 시작 앵커를 발급하기 전에는 실제 Boot를 재시도하지 않는다. 이후 허용될 한 번의 시작은 New0으로 끝난 이전 키 전 실패와 구별한다. 새키 한 세대·45분·Bind1/Sign1·폐기·서명 전 실제 계획 검토와 별도 Authorize 관문은 그대로다.
