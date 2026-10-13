# C4 C2 미호출 한정 규칙 테라 자체 검사

- 작성: 2026-09-30, 테라 구현 담당, 실제 `gpt-6-sol`. 자체 검사이며 독립 수용이 아니다.
- 근거: `2026-09-30-c4-pre-c2-failure-evidence-amendment.md`, Approved SHA `7F1BAA3821580E7382258B7C8BD0F045818FF51B9334AD0C905EB7B9A6388356`.
- 추적: REQ-M5D7QC4-001/003/005/007, AC-M5D7QC4-001/003/005/007/009/010.

생성기는 기존 공유 `C4-ExpectedRow`를 사용한다. 검증기에 memoryGeneration 전용 `C2NotInvokedAfterIssuedExecution` 분기를 추가했다. 네 이름·실제 Int64 값8/9/10/11, RequiredReached, IssuedWithoutResult 전체 원본 진단, AfterConsume, checked=false, outcome/phase=null, 실제 호출 수1/0, memory null·proof Unknown 및 각각 Unknown certainty, 동결 소스 경계와 정확 추가 진단 순서를 검사한다. 기존 세대 규칙과 정상 Unknown 거절 조건은 유지한다.

최종 국소 검사 실행의 실제 종료값은0, 판정 일치66/66이다. 정상 네 경계4개와 음성62개다. 음성은 경계 이름·값, 미도달, consume 전, 결과 발급, checked, memory non-null, outcome/phase, proof Absent/SameActual, C1=0 및 C2/observer/fresh 호출, 원본 thread/pair/CWT/fault 진단, 추가5진단의 누락·값·certainty, source 참조 부재/다른 경로, canonical certainty, 증거 종류·규칙 경로 및 예상 호출 사실의 certainty·값 불일치를 포함한다.

처음 조건 검사52/52의 원시 자료를 보존한 뒤, 예상 canonical 사실 결속·계수기 certainty 검사14개를 추가했다. 최종66개 원시 자료는 별도 생성했다. 기존 발급 세대33개와 추가5개 기록은 변경하지 않았다. 두 도구와 국소 검사 스크립트의 PowerShell 구문 오류는 각각0이다.

이 검사는 행 규칙의 조건 함수에 합성 입력을 직접 전달한다. 앞36자리 일부는 범위 밖 자리표시자이며 전체 canonical 행·실제 NUnit XML·실제 source SHA 결속을 통과했다는 자료가 아니다. Source 참조의 정확 경로·형식은 검사하지만 실제 실행 입력 SHA 대조는 정식 `C4-MatchTerminal`의 기존 `C4-Pointer` 단계에서 수행한다. 실제 CWT 원본, 예외 참조 동일성, callback 도달 및 동결 본문 지배는 fixture 발행기와 독립 검수가 입증해야 한다. 합성 일치만으로 실제 권한을 발급하지 않는다.

| 파일 | SHA256 |
| --- | --- |
| artifacts/c4-build-required-checkpoint-rows.ps1 | 14A1D52C8BB2113DE5708C8BB0BE1ACC978A6486D2F5608E0096485F398A723E |
| artifacts/c4-verify-required-rows.ps1 | 6F34AD4612AFE3D7228C39D1AEA2A0260D6EC0E062C1AD1A5D712DF7DAA4EA5F |
| docs/verification/2026-09-30-c4-pre-c2-terra-localcheck.ps1 | 52C58993739982C8458F416C8D024E85199C91F88A11B21FE0749F51A23823AA |
| docs/verification/2026-09-30-c4-pre-c2-terra-localcheck.json | 5E9E98FA78B23F0B9E229F36DE8CCF91557A3E528102C40E9C177AEE527C9BB1 |
| docs/verification/2026-09-30-c4-pre-c2-terra-localcheck-r2.json | 7FF86C409F82C19D4A9F0D4D20D9ED00742F3D47895B2669B54B9C6E6356D5D4 |

컴파일·Unity·runner·queue 실행 및 최종 원장 발급은 하지 않았다. WholeAccepted=false이며 실제 실행·독립 검수·아스트라 최종 수용은 남아 있다.
