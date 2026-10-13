# C4 미발급 세대 도구 국소 음성 추가 검토

- 검토자: GPT-6 Luna, 2026-09-30.
- 원 검토: [도구 독립 검토](2026-09-30-c4-issued-generation-tools-luna-review.md), SHA는 `docs/verification`의 현재 파일 지문으로 확인한다.
- 판정: 이 국소 회귀 범위의 P1은 폐쇄 가능하다. P0 0 / 남은 도구 국소 P1 0.

원 검토에서 승인 규격이 요구하는 잘못된 진단값 개별 거부 다섯 건이 구현자 원래 33건 표에 없음을 지적했다. 원래 스크립트·JSON은 보존됐고, 이번 추가 표는 다음 다섯 진단 각각의 값을 `WrongValue`로 바꾸어 거부되는지 분리 확인한다.

- `executionGenerationSameActualCwt`
- `executionConsumeState`
- `executionFailureObserved`
- `memoryGenerationSameActualCwt`
- `memoryReceiptIssuePathSourceEstablished`

추가 원시 JSON은 5건 중 5건 기대한 거부, 검사 종료값 0, `SyntheticLocalOnly=true`, `ActualUnityExecuted=false`, `WholeAccepted=false`로 기록한다. 독립적으로 원시 JSON의 다섯 검사명·기대/관측 거부·일치 표지를 대조했다. 행 생성기와 검증기 SHA는 각각 `06B484BD4CF5815B03D73D4AEDE0958C0CAF8A1DE4450F42411CF5F7A868F338`, `57F89ED5BD7D4BF004693BFCEFE0E4A0B10D0B421A151413364E90054476BD5C`로 원 검토와 동일하다. 추가 스크립트/JSON/설명 SHA는 각각 `C2AE219FF53E9C05297CF7E1CF8D3042121C814D8E6AA54CD0373FE7F7269F9C`, `A4C214C587D4508E74BE315CE04D00572A7B341F532EDB7294A0B6F7C3821090`, `319AC19AB46243FEE29280B2D417328AFADF5F2F1FCC910B7D313FCBD9F9FD10`이다.

폐쇄 범위는 다섯 합성 진단값 변조 사례뿐이다. 실제 CWT 원본·thread·발급 이력·영수증 Validate를 올바르게 관찰하고 진단 발행을 지배하는지는 fixture emitter의 독립 정적 검수와 실제 실행에 남아 있다. 따라서 도구 단위 국소 P1 폐쇄는 C4 런타임/fixture/Unity 실행 수용 또는 전체 수용을 뜻하지 않는다.
