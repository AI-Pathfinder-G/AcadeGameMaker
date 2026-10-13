# Q0 증거 정수 정밀도 보정

상태: **Approved** — 2026-10-03 실제 `gpt-6-astra` 승인. `REQ-PLAT-061/062/068/069/070`, `AC-PLAT-059/060/066/067/068`의 실제 프로세스 신원·증거 결속을 보정한다. 소스/키/실행을 다시 만들지 않고 보존된 원시 관측에서만 정밀도를 복구한다. 이번 승인은 준비 수용이나 운영 개시가 아니다.

## 확인된 P1과 불변 실패 원천

실제 아스트라가 공통 Unity 문서와 원문01~04 영수증의 숫자를 직접 대조하여 전체 ticks 7곳의 불일치를 확인했다. JavaScript Number를 거친 내장 ObservedReceipt의 값이 원문 Int64와 다르다. 테라가 보존 원문 수집 envelope에서 확인한 채널 StartTicks는 `639265539138890254`이고 기존 공통 문서는 `639265539138890200`이다. 채널 원시 stdout3131바이트/Base64/지문, 원문 Unity 영수증과 로그는 별도로 보존되어 있다. 원문 관측의 성공과 정밀도 손실 공통 문서의 부적합을 구별한다.

다음 네 파일은 삭제·덮어쓰기·성공 수용하지 않고 FailPrecisionSources로 결속한다.

| 경로 | 원시 SHA-256 |
|---|---|
| output/q0-trusted-child-evidence-r1/channel-tests.json | D6510D6FC966FAAA10720DA6F461AD474F6EB201D89B7D288D6AFF78C753DD3C |
| output/q0-trusted-child-evidence-r1/unity-compile.json | 84DBCAD74D63EBFB357C37879389F025CE13E7C02DEBB324817381A9C764C333 |
| output/q0-trusted-child-evidence-r1/preparation.json | 5EFEA79F4E89652429CD24F1FDA42203D1D1DF5426DEE95323F4A4E6A16BD618 |
| docs/verification/2026-10-02-q0-trusted-child-terra-report.md | 58B9EAE569EBD4131077FA55516672B92733B52086CDE60C1597B0CF595C423D |

앞선 파일의 P0/P1=0 등 자체보고는 발견 전 주장으로 보존하며 이번 정밀도 P1을 닫은 근거로 쓰지 않는다. 원시03 실패·첫 채널 수집 MissingRaw/UnknownExit도 계속 보존한다.

## 정확 새 경로와 소유

허용 추가 파일은 아래 다섯 개뿐이다.

- `output/q0-trusted-child-evidence-r1/channel-tests-v2.json`
- `output/q0-trusted-child-evidence-r1/unity-compile-v2.json`
- `output/q0-trusted-child-evidence-r1/preparation-v2.json`
- `docs/verification/2026-10-02-q0-trusted-child-terra-report-v2.md`
- `docs/verification/2026-10-02-q0-trusted-child-luna-review-v2.md`

테라는 세 증거와 자기 보고, 루나는 별도 독립 보고를 작성한다. 현재 부재인 원래 luna-review.md 대신 정확 v2 경로를 사용하며 중복 보고를 만들지 않는다. 최종 수용은 아직 부재인 기존 `docs/approvals/2026-10-02-q0-trusted-child-limited-acceptance.md`에 실제 아스트라가 두 보고 검토 후 한 번 작성한다. 임의 초안 파일·추가 실행/수집·Unity 재시도·소스 수정은 승인하지 않는다.

## 손실 없는 복구와 발급 전 검사

채널에는 `RawCollectorEnvelope={ByteLength,Sha256,Base64}`로 테라 세션에 실제 보존된 수집기 원문 UTF-8 전체바이트를 넣는다. 기존 파싱 객체를 다시 JSON으로 만들어 원문이라 부르지 않는다. 원문이 없으면 재구성하거나 ticks를 추측하지 않고 실패한다. 원문 stdout/stderr, 실제 종료,71개 사례·14사건, 소유 신원과 전후 소스는 그 envelope 및 이미 보존된 raw stdout과 대조한다.

원시 문서→Int64 지원 파서→PowerShell/.NET 직렬화 경계만 사용한다. JavaScript는 원문 문자열/바이트의 보관·전달에만 사용하며 큰 정수를 Number로 파싱한 객체의 재직렬화는 금지한다. 중첩 객체까지 모든 정수 토큰을 열거하고 원문 decimal token과 Int64의 왕복 문자열을 대조한다. 부호/범위·소수/지수·문자열숫자·중복키·엄격 UTF-8을 확인한다. 2^53 초과 값과 모든 ticks는 별도 전수 목록으로 원문/결과 차이0을 확인한다. 정확 숫자를 반올림하거나 바뀐 숫자를 문자열로 바꾸는 것으로 복구하지 않는다.

Unity 내장 영수증은 각각의 해시 결속된 원문 파일을 다시 직접 읽는다.01/02/04 각 UnityStartTicks와 NativePipePeerStartTicks,03 UnityStartTicks 등 확인된7곳뿐 아니라 전체 중첩 숫자를 검사한다. 시작 ticks와 실제 프로세스 StartUtc의 의미 일치도 확인한다. 수집 시작시각과 프로세스 시작시각은 서로 다를 수 있으므로 같은 이름/뜻으로 바꾸지 않는다. 원문에 프로세스 UTC가 없으면 정확 ticks로 계산한 UTC는 파생값이라고 명시하고 원시 관측이라고 꾸미지 않는다.

완성 후보는 메모리에서 원시바이트로 직렬화하고, 원문 바이트 결속·전체 정수·상호SHA·현재570 지문/길이·공개4자리 UNISSUED·실패 이력·거짓 운영/전체수용 플래그를 전수 검사한다. root와 루나가 해당 후보 원문을 읽을 수 있게 보존하여 발급 전 정밀도 확인을 거친다. 검사 전에 CreateNew로 최종을 먼저 만들지 않는다. 채널/Unity 후보SHA를 준비 후보에 결속한 뒤 동일 검사된 바이트를 각 v2 경로에 한 번 CreateNew한다. 이 후 테라 보고, 실제 발급 바이트와 원문을 독립 대조한 루나 보고 순서로 작성한다. 실패하면 기존 결과를 보존하고 v3를 자동 생성하지 않는다.

## 계획 결속과 남은 관문

preparation-v2의 ChannelTests/UnityCompile은 정확 v2 경로/원시SHA를 가리킨다. 향후 계획 CurrentPreparationEvidence는 기존 두 역할 그대로 Preparation→preparation-v2.json, NegativeTests→channel-tests-v2.json이다. schema 필드/역할 수를 바꾸지 않는다. 준비→unity-compile-v2 결속과 FailPrecisionSources의 네 지문을 실제로 확인한다.

현재570 소스·원문04 실제 지원 결과는 보존하되 새 증거/두 보고·아스트라 수용 전 공개 리터럴을 치환하거나 운영키·실행 복제·Git·원격·Authorize를 시작하지 않는다. 이번 보정에 새로운 시행이 없다는 점과 실제 테라/루나/아스트라 참여 범위를 구별하여 보고한다. 전체 역사 P1·실제 서명/아홉 선택·게임 게시 관문은 미수용 상태를 유지한다.
