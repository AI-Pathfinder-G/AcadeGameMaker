# C4 결과 미발급 세대 규칙 자체 국소 검사

- 작성: 2026-09-30, 구현 테라, 실제 `gpt-6-sol`.
- 근거: Approved 개정 `docs/specs/work-contracts/2026-09-30-c4-issued-generation-evidence-amendment.md`, SHA `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-001/003/005/007/009/010`, `AC-M5D7QC3-007/008`.
- 자체 합성 조건 검사이며 독립 수용이 아니다. 실제 Unity·컴파일·runner·queue·최종 원장 발급은 수행하지 않았다. `WholeAccepted=false`.

`c4-verify-required-rows.ps1`에서 두 새 규칙의 정확 generation 경로, 결과/phase null, checked false, 사전 진단 순서·값·certainty, receipt의 execution 규칙·C1/C2 각1·observer0/1·출처/검사상태 조합을 검사한다. canonical36과 기존 Positive/Absent/Unobserved 분기 본문은 유지했다. 생성기는 기존 공유 `C4-ExpectedRow`를 호출하므로 동일 검사 경계를 사용하며 별도 완화 규칙을 만들지 않았다.

실제 국소 실행 결과는 33건 중 33건이 기대와 일치했다. 정상 조건 4건은 execution 발급, observer 전달 전 receipt, observer 전달 후 Rejected/Validated receipt다. 음성 29건은 필수 진단13개 각각 누락, 숫자0/음수/문자열/실수4개, 원본 thread/발급 association/미발급 결과/영구 fault/receipt Validate/원본 session pair 진단 실패6개, checked true, observer 불일치, source/actual certainty 바꿈2개, 잘못된 execution 규칙, 잘못된 generation 경로다. 실행 프로세스 종료값은 0이었다. 두 변경 도구의 PowerShell 구문 오류는 각각0이며 기존 strict Structure 예상 행5개를 공유 예상 행 검사에 다시 통과시켰다.

검사 스크립트와 각 입력/관측/예외를 포함한 원시 결과는 같은 디렉터리의 `2026-09-30-c4-issued-generation-terra-localcheck.ps1`과 `.json`으로 새 파일 생성 방식으로 보존했다. 합성 자료의 첫36개는 국소 helper를 격리하기 위한 명시적 placeholder이며 실제 canonical 행이나 NUnit 결과로 발급하지 않는다. 양수 Int64만을 일관되게 꾸민 값과 실제 원장 값의 동일성은 JSON 조건 검사만으로 증명할 수 없다. 실제 CWT·readonly 발급 기록·정상 receipt Validate가 해당 진단 발행을 지배하는지는 fixture emitter의 별도 독립 검수와 실제 실행의 대상이다.

변경 도구 SHA:

- `artifacts/c4-build-required-checkpoint-rows.ps1`: `06B484BD4CF5815B03D73D4AEDE0958C0CAF8A1DE4450F42411CF5F7A868F338`.
- `artifacts/c4-verify-required-rows.ps1`: `57F89ED5BD7D4BF004693BFCEFE0E4A0B10D0B421A151413364E90054476BD5C`.
- 원시 결과 JSON: `A3D51D15F14EA467C369450F3517EBB362A64A8062D7B4E53149293B3637BBDF`.
