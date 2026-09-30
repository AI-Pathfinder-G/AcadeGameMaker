# C4 QA 증거 규약 R2 독립 검토

검토 대상 `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md`의 현재 SHA-256은 `F7B07E26C45C9B0DCD4CAE6915BAF4D0D8DD5CBE30E0D1D5D3878C060A2674D0`이다. R1 원문은 별도 `2026-09-29-c4-qa-evidence-protocol-r1-draft.md`에 SHA `A883D0AF0FAC7D62BA5840676EB09377EC9F59CADAAC3F7DE770F248F932BCB1`로 보존되어 있고, R4 소유 계약 `EF54AE99EEF6E482C267C949A0C1B53709FF19611E9791300E07764EBD9256C0`는 바뀌지 않았다.

판정: P0 0, P1 0. 이전 검토 `2026-09-29-c4-qa-evidence-protocol-consolidated-luna-review.md`의 RequiredFacts와 실제 중첩값 결속 P1은 R2 문구로 폐쇄 가능하다.

R2는 ExpectedRows에 `ExpectedAuthorityCorrelation`, `ExpectedGuard`, `ExpectedBarrier`, `ExpectedHistory`, `ExpectedCleanup`을 별도 정확 키로 두고 타입을 정했다(59, 140–162행). 비교기는 다섯 nested 객체의 전체 leaf를 정확 비교하고, 별도로 terminal `after.facts`의 정규 경로를 nested 실제 leaf와 일대일 대조하며, RequiredFacts 배열 전체도 이름·순서·개수·고유성·값·certainty로 비교한다(195–197, 240, 246행). 중첩값과 fact가 다르면 실패하고 nested Unknown/미검증을 facts만으로 통과시키지 않는다는 의무까지 명시됐다(246–248행). 따라서 단순 fact marker, 중복·누락 사실 또는 before snapshot 대체로 정상 결과를 만들 수 있던 해석 공백은 닫혔다.

정규 경로 36개 수는 실제 고정 스키마와 일치한다. `authorityCorrelation` 12개, `guard` 3개, `barrier` 3개, `history` 5개, `cleanup` 6개, `callCounts` 7개로 총 36개이며, 실제 키 선언과 순서(53–57행)에 대응한다. RequiredFacts의 terminal 전용 범위와 경과 레코드 배제(197행), 36개 경로의 정확한 이름·순서·1회 존재(199–238행), JSON leaf의 정규 문자열 변환(240행)이 서로 모순되지 않는다. 실행 세대 두 값은 Positive/Absent/Unobserved 규칙으로 제한되고, Unknown/부재를 관측된 정상 권한으로 올리지 않는다(164–182행). 동적 진단 규칙도 barrier 증거 참조와 cleanup 예외 두 경로에만 한정되어 있다(184–193행).

이 검수는 규범 설계의 정적 검토다. 문서는 계속 Draft이며 새 emitter, ExpectedRows 원장, verifier 구현 또는 실제 실행은 없다. 따라서 이 결과는 구현 일치, Unity 시험 통과, C4 수용 또는 승인 권한을 부여하지 않는다. 원본 R1/R2 계약과 소스·QA 도구를 변경하거나 Unity/Git 명령을 실행하지 않았다.
