# C4 정확 개정 계약과 결과 구성 설계의 제한 수용

2026-09-29, 아스트라, 실제 `gpt-6-astra`. `REQ-M5D7QC4-001..007`, `AC-M5D7QC4-001..010`의 계약 준비를 제한 수용한다. 이 기록은 C4 구현 승인이 아니며, 현재 Review와 C3 마지막 필수610·선행 gate 미완료를 유지한다.

수용 대상은 다음 정확 문서의 조합이다.

- [정확 개정 계약 Draft](../specs/work-contracts/2026-09-29-c4-r3-exact-implementation-amendment-draft.md), SHA `ACC8F20DE5D4D4E21F1385C5E204DC2D79BFB76F35CE12F7A439DC3599217738`.
- [새 확인 전달 API와 결과 구성 r2](../proposals/2026-09-29-c4-r3-fresh-api-and-result-map-r2-draft.md), SHA `B1B5A36757595F9DB993950819A61DB72ECBE44FFBDB369CA45D5C674FED50F6`.
- [r3 규범 선택의 제한 수용](2026-09-29-c4-r3-normative-design-limited-acceptance.md), SHA `178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692`.

루나의 [정확 개정 및 초기 결과 구성 검토](../verification/2026-09-29-c4-r3-exact-amendment-and-fresh-map-luna-review.md), SHA `05314226F889A99A9B29CAE30EAD21EA3C292CDEE7903019E3C5BAB7C7A1A705`는 원본 결과 분류와의 충돌 P1 한 건을 기록했다. 초기 C401 보완안과 이 지적은 이력으로 보존한다. [r2 독립 재검수](../verification/2026-09-29-c4-r3-fresh-map-r2-luna-review.md), SHA `7BD27130BA00E1874C5F6328BE3A47B61DFA50DE14194FDE2FB9BFE05B7C2549`, 정적 P0/P1=0에서 해당 지적이 해소됐음을 확인했다.

원본의 다섯 outcome 및 네 phase를 그대로 선택한다. Busy와 ConfirmationStale는 서로 다른 결과이며 `FreshC3Required`는 가드·생명주기 상태다. 기본값0·미정 enum·closed nullable 조합 밖의 값은 거절한다. 실제 C1 typed row를 받지 못한 예외에는 정상 결과를 발급하지 않는다. C1 terminal은 C1Returned, C2 비완료는 C2Invoked, 성공은 Completed 단계다. C2 Busy를 terminal Reload 안내로 조합해도 실제 C2 Busy 원본은 보존한다.

하위 계층의 네 전달 API는 실제 원본 결과·예약·Owner 참조·pair·다음 token에 결속한 불투명 권한을 다룬다. 허브의 acknowledgment와 단일 consume가 lower Complete 호출을 지배해야 하며, 조기 호출 경로나 예약의 외부 유출은 허용하지 않는다. 하위 계층이 허브 상태를 역조회하는 호출이나 proof/root 추출 API를 추가하지 않는다. 결과의 모든 getter는 전체 enum/null/pair/root/generation/nested 원본 상관을 검증하고 forensic 읽기를 현재 권한으로 사용하지 않는다.

정확 runtime·시험·fixture·메타·감사 보정·신규 QA 도구 허용 후보와 비순환 동결 절차를 선택한다. Q0 pin 자원은 변경된 Adapter/Router의 exact 바이트만 연결하고 자기 지문을 포함하지 않는다. pin→Q0→source manifest→선택·행→입력 목록→실행 계획→실제 결과 순서를 유지한다. legacy Q-B/C3 successor/Q-A scope 감사는 변경하지 않는다. 새 Q-A/Q-B 동작과 lower 완료 지배 구조는 신규 엄격 감사와 독립 실제 검수의 대상이다.

현재 수용은 기술 계약 설계에 한정된다. C3 선행 gate와 정확 C4 Approved 전환 전 Terra 구현·신규 QA 도구 작성·Unity 실행을 허용하지 않는다. 구현 뒤에는 신규 소스 동결·정식 이름/count·checkpoint 행·변경 소스의 집중 및 필수 실제 회귀·루나 독립 검수·아스트라 통합 수용이 필요하다. R11 고정14·입력884나 과거 통과를 새 소스 수용에 재사용하지 않는다. C3 AC007/008, C4 전체, 게임 코드 최초 게시의 다섯 차단은 이 기록으로 닫히지 않는다.
