# C4 실행 검증기의 XML 결속 보정 재검토

검토 대상 `artifacts/c4-verify-run.ps1` SHA-256: `EEE8F4E22A107D54776589E706746327FC6A7DCF1D49F52603D768F8474E2BFC`. 이전 검토 대상 `FA610EABEAAA3FD6DB36B32F7BFA3B7B96E4F1F22791B1F4283231507B9EB414`의 지적은 행 비교 결과 XML과 실제 실행 XML의 경로·지문을 맞대지 않는 것이었다.

현재 본문은 실행 계획의 `$out.Xml`에서 `$expectedXmlBinding`을 만들고, C4 행 비교 분기에서 기존 `C4-CheckBinding` 이후 `row.XmlBinding`과 이 결속 객체를 정확 비교한다. 비교 실패는 검증을 중단한다. 최종 검증 보고서의 `XmlBinding`도 같은 `$expectedXmlBinding`을 사용한다. 따라서 다른 기존 XML을 참조하거나 같은 경로의 SHA가 다른 행 비교 결과는 통과할 수 없으며, 이전 P1은 정적으로 닫혔다고 판단한다. 기존 Matrix91 legacy 분기는 그대로 `C4-LegacyRows`를 통해 실제 `$out.Xml`을 대조한다.

구현자 측 국소 조건 점검은 동일 XML 허용, 다른 기존 XML 및 같은 경로·다른 SHA 거절 4/4를 보고했다. 이는 구현자 자체 점검이며 독립 실행은 아니다. 이 검토에서는 스크립트 구문 오류 0 및 위 조건 비교를 정적으로 확인했다. Unity/큐/실제 NUnit 실행은 하지 않았으므로 운영 성공이나 전체 QA 수용으로 확대하지 않는다.

정적 판정: P0 0, 이 XML 결속 보정에 남은 P1 0. 전체 운영 도구 검토의 다른 조건부 입력목록 지적과 미실행 한계는 기존 독립 검토 기록에 그대로 남는다.
