# 원격 도구 입력 검토의 tree 지문 정정

2026-09-29. 이 보완 기록은 [R1 독립 검토](2026-09-29-c3-initial-publication-remote-inputs-luna-review.md) SHA-256 `E31DA91CDE56D5FA397AAEA03D7FEE833B967EAB6F6DAC0199259C24C8972E3A`의 tree 지문 판정을 보완한다. 기존 검토와 R1 증거는 보존한다.

R1 tree 캡처의 `sha`는 commit SHA와 같아 tree 식별 오류로 판정했다. 그때 독립 재구성한 값 `4e58381525ef9e933a45d61d8473278b60e896ff`도 잘못이었다. 계산 중 디렉터리 항목의 정렬용 `/`를 객체 저장 이름에도 붙였기 때문이다. Git tree 객체에는 디렉터리 이름을 `/` 없이 저장하고, `/`는 정렬 비교에만 사용한다.

상위가 새로 수집한 commit-object R2 SHA-256 `A5F37A0203BABDD3826F932D1D6CFC47C6BE4300B180AF2186EC1606909CA7CB`에는 commit `4d864484f75939dcf9c93c055d296613cf1ba1dd`와 tree `5de182aa5ed735755ed9ac9ee4fe071eac828381`가 분리 기록돼 있다. tree R2 캡처 SHA-256 `A96A0BA1AEE1739BA15208BDC8FFDFB66016B96D4C2463A9C262BFFC692A26A8`도 `sha=5de182…`를 가리킨다. R1·R2 tree의 905개 경로·mode·type·blob/tree 지문 차이는 0개다. 제공된 최상위 항목을 디렉터리 mode의 선행 0을 제거하고, UTF-8 바이트 정렬 키의 디렉터리 `/`와 저장 이름을 구분하여 다시 구성한 tree 객체 SHA-1도 `5de182aa5ed735755ed9ac9ee4fe071eac828381`로 일치한다.

새 비교 원장 `artifacts/c3-initial-publication-remote-input-comparison-r2.json` SHA-256 `660BA3BBA0BC7BA5777376A8F2B1CD0ED0A1545EC88E879C9BD22C25C55BD4D8`는 R1을 보존하고 commit·tree 지문 연결만 새 캡처로 구분한다. 905개 항목은 동일하며 7개 입력 결과는 원격 존재 3, 부재 4, 바이트 동일 1, CRLF/LF만 다른 경우 0, 불일치 2로 유지된다. `RawInputComparisonPassed=false`, `PublicationApproved=false`, `ReproductionProved=false`, 누락 입력 유지도 그대로다.

**정정 판정: R1 tree 포인터 메타데이터 P1은 R2에서 닫혔다.** 이전 계산의 오류는 본 검토의 재구성 방식이었고, 원격 캡처와 R2의 canonical 재구성은 같은 tree SHA에 일치한다. 다만 최초 게시의 도구 입력 7개 차단은 닫히지 않았다. 네 입력 부재와 두 원시 바이트 불일치가 남으므로 원격 존재·구조 비교만으로 게시 승인이나 재현을 주장하지 않는다. 이 검토자는 R2의 상위 조회 기록과 로컬 지문을 읽었으며 네트워크나 Git을 실행하지 않았다.
