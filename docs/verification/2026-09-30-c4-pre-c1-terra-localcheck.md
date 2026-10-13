# C4 C1 미호출 규칙 테라 자체 검사

- 작성: 2026-09-30, 테라 구현 담당, 실제 `gpt-6-sol`. 자체 검사이며 독립 수용이 아니다.
- 근거: Approved `2026-09-30-c4-pre-c1-failure-evidence-amendment.md` SHA `8B312BF1374B9B8A0164D4937A9D84ECAF31C69B8058DB7D4CEB64B1D6339F1D`.
- 추적: REQ-M5D7QC4-001/002/003/006/007, AC-M5D7QC4-001/002/003/006/008/009/010.

`C1NotInvokedAfterIssuedExecution`을 memoryGeneration 전용으로 추가했다. 정확 C4 이름·값1..7, 첫 경계 BeforeConsume·나머지 AfterConsume, 같은 IssuedWithoutResult 원본 진단, 모든 C1/C2/observer/fresh 수0, proof Unknown 및 memory null과 Unknown certainty, 동결 C1 이전 source 지배·장벽 pointer, 추가6진단 순서를 요구한다. 기존 C2 미호출4경계와 Positive/Absent/Unobserved/receipt 규칙은 그대로 둔다.

국소 검사 실제 종료값0, 판정 일치76/76(정상7, 음성69)이다. 음성은 각 경계 이름·값, 소비 시점 양방향, 미도달·다른 EvidenceKind/family, 결과 발급·checked·memory non-null·proof Absent/SameActual, 여섯 호출 수 양수·certainty, CWT/원본 thread/발급/fault 진단 결여, 새6진단 누락·값·certainty, source pointer 누락·오경로, 장벽 source 계획 부재·잘못된 kind를 포함한다. 처음74개 자료는 보존했고 장벽 근거 반례2개를 추가한 최종 자료를 따로 생성했다.

조건 함수에 전달한 합성 자료이며 앞36 canonical 자리 전체를 형식 검증한 행은 아니다. 실제 CWT, callback 예외 동일성, 원본 소비·장벽·정리는 fixture와 독립 검수에서 입증해야 한다. Unity·runner·queue·원장 발급은 하지 않았다. WholeAccepted=false다.

| 파일 | SHA256 |
| --- | --- |
| artifacts/c4-build-required-checkpoint-rows.ps1 | DBD8BE999330657B81489342BB58586B0952583AF0867C591DB5E39949A9C071 |
| artifacts/c4-verify-required-rows.ps1 | DEF2391963186E331EE1EA3C04BDF63E21D8DE3F4E271222CC15C2E865F6065B |
| docs/verification/2026-09-30-c4-pre-c1-terra-localcheck.ps1 | E3F5430E0845532C73E41C1221081E81342BFB244F93B8BF038B7DAC7FF7942F |
| docs/verification/2026-09-30-c4-pre-c1-terra-localcheck-r2.json | 6B6731DEBF1AFA93392A1F52DED41299CB4469970199C672DB315D0C41973F9D |

두 도구와 국소 스크립트의 PowerShell 구문 오류는 각각0이다. 실제 source 동결·행 전체·부모 NUnit·독립 수용은 남아 있다.
