# C4 실행 검증 XML 결속 테라 자체 검사

- 작성: 2026-09-30, 테라 구현 담당, 실제 `gpt-6-sol`. 자체 검사이며 독립 수용이 아니다.
- 추적: REQ-M5D7QC4-007, AC-M5D7QC4-010.
- `artifacts/c4-verify-run.ps1` 수정 전 SHA `FA610EABEAAA3FD6DB36B32F7BFA3B7B96E4F1F22791B1F4283231507B9EB414`; 수정 후 SHA `EEE8F4E22A107D54776589E706746327FC6A7DCF1D49F52603D768F8474E2BFC`.

기존 행 비교 XML 결속의 실제 파일 SHA 확인 뒤 그 경로·SHA가 해당 실행 `OutputPaths.Xml`의 실제 결속과 동일한지 추가 검사한다. 최종 검증 결과의 `XmlBinding`도 그 같은 결속 객체를 사용한다. 기존 Matrix91 legacy 분기와 원본377 검사는 그대로 유지한다.

실제 운영 도구의 해당 조건문을 추출하여 두 기존 원시 XML로 국소 검사했다. 정상 동일 XML 수용1, 다른 기존 XML 결속 거절2, 같은 경로의 다른 SHA 거절1로 실제 종료값0·판정 일치4/4이다. 운영 도구 구문 오류0. 국소검사는 전체 실행 검증을 호출하거나 새 실행 증거를 만들지 않는다.

| 파일 | SHA256 |
| --- | --- |
| docs/verification/2026-09-30-c4-verify-run-xml-terra-localcheck.ps1 | 17BC2EA7F1C244CA012189DD1ACBC0022C7C0F2460E5CB539441EBBE920600EA |
| docs/verification/2026-09-30-c4-verify-run-xml-terra-localcheck.json | 1F0D030EB4BCC9C9F749218F026CADC624A13C74A37AF802EDF32BF7E1612D5D |

Unity·runner·queue·최종 원장 발급은 하지 않았으며 전체 수용은 남아 있다.
