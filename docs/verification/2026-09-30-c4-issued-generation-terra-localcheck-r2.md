# C4 세대 진단 값 추가 자체 음성 검사

- 작성: 2026-09-30, 구현 테라, 실제 `gpt-6-sol`.
- 근거: Approved 발급 세대 개정 SHA `FF88BFC0471FB48C2828765A1C6FBD0F3913F0DD123CDEB16BCBADD4F8F2DCB6`.
- 추적: `REQ-M5D7QC4-001/003/005/007`, `AC-M5D7QC4-001/003/005/007/009/010`, `AC-M5D7QC3-007/008`.
- 자체 합성 조건 검사이며 독립 수용·실제 CWT 증거가 아니다. 실제 Unity·컴파일·runner·queue는 실행하지 않았다. `WholeAccepted=false`.

원래33건의 검사 스크립트와 원시 결과는 그대로 보존했다. 정확 원래 함수 두 개를 해당 스크립트 SHA `63D96EFCA0D1F1679F9EB1D855823AADDB660342A92FCA3C01268EFCDA265116` 검증 후 읽고, 정상 합성 조건에서 다음 다섯 진단의 실제 값을 각각 `WrongValue`로 바꿨다. 모두 거절돼 실제 추가 실행 결과5/5, 프로세스 종료값0이었다.

- `diagnostic.executionGenerationSameActualCwt`
- `diagnostic.executionConsumeState`
- `diagnostic.executionFailureObserved`
- `diagnostic.memoryGenerationSameActualCwt`
- `diagnostic.memoryReceiptIssuePathSourceEstablished`

추가 검사만 수행했으며 두 도구 바이트는 수정하지 않았다. 원시 입력·관측·거절 예외는 새 `2026-09-30-c4-issued-generation-terra-localcheck-r2.json`에 생성했다. 숫자와 진단을 일관되게 꾸민 JSON만으로 실제 원장 동일성을 입증할 수 없다는 기존 한계는 동일하다. actual CWT/readonly 발급 원본·정상 receipt Validate의 진단 발행 지배는 emitter 독립 검수와 별도 실제 실행의 대상이다.

- 추가 검사 스크립트 SHA: `C2AE219FF53E9C05297CF7E1CF8D3042121C814D8E6AA54CD0373FE7F7269F9C`.
- 추가 원시 결과 SHA: `A4C214C587D4508E74BE315CE04D00572A7B341F532EDB7294A0B6F7C3821090`.
- 생성기 SHA 유지: `06B484BD4CF5815B03D73D4AEDE0958C0CAF8A1DE4450F42411CF5F7A868F338`.
- 검증기 SHA 유지: `57F89ED5BD7D4BF004693BFCEFE0E4A0B10D0B421A151413364E90054476BD5C`.
