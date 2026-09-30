# C4 r3 정확 개정·fresh 결과 map 독립 검토

검토 자료와 확인 SHA-256:

- `docs/specs/work-contracts/2026-09-29-c4-r3-exact-implementation-amendment-draft.md`: `ACC8F20DE5D4D4E21F1385C5E204DC2D79BFB76F35CE12F7A439DC3599217738`
- `docs/proposals/2026-09-29-c4-r3-fresh-api-and-result-map-draft.md`: `C401ED97EEE7FA43CEB8CB40790AA952863FE721D1DD97FBD98112D4AC56089E`
- 한정 규범 수용 `docs/approvals/2026-09-29-c4-r3-normative-design-limited-acceptance.md`: `178A06597266878D01B5D2CBBC82EC2C4250A489BFD4CE09690711EDF6A3A692`
- 원본 Review 계약 `docs/specs/work-contracts/2026-09-28-vd09-m5d7q-c4-reset-execution-bridge.md`: `3F29B8BC7D3F0172717C0FC94101BF4086FEE1FE475B65A3B2E67E0AE4FA5DC0`

**판정: P0 0건, P1 1건.** 정확 개정과 fresh map은 Draft이며 구현·실행 권한이 없다. 가장 큰 차단은 새 result Outcome/Phase enum이 원본 Review 계약의 closed output을 변경한다는 점이다. 규범 한정 수용 178A가 기술선택만 승인했고 C4 closed output을 바꾸지 않았으므로, 해당 map은 현재 계약과 일치하지 않는다.

## P1 — 기존 C4 closed output 보존 필요

원본 C4 계약 182~192행은 Outcome을 `Completed`, `Busy`, `ConfirmationStale`, `ReloadRequired`, `ManualRepairRequired` 다섯 값으로 닫고 Phase를 `PreC1`, `C1Returned`, `C2Invoked`, `Completed` 네 값으로 닫는다. 실제 C1 Busy와 ConfirmationStale는 모두 `FreshC3Required` 전이를 유발하더라도 caller가 받는 execution result는 각각 정확한 `Busy` 또는 `ConfirmationStale`여야 한다. 원본은 둘의 typed C1 행과 fresh handback을 각 result에 보존한다.

새 map은 Outcome을 `FreshC3Required`, `Completed`, `ReloadRequired`, `ManualRepairRequired` 네 값으로 축약하고 Busy와 ConfirmationStale를 단일 `FreshC3Required/NoBarrierFresh` 행으로 합친다. Phase도 `NoBarrierFresh`, `DiskPreparedCutover`, `TerminalC1Outcome` 세 값으로 대체한다. 특히 C2가 실제로 Completed인 결과를 `Completed/DiskPreparedCutover`로 두어 원본의 terminal `Completed` phase 의미도 재정의한다. C1 원본 row를 사적으로 보존한다고 해도 contract-visible 결과의 닫힌 분류·단계를 바꾼 사실은 해소되지 않는다.

최소 보정은 원본 두 enum의 정확한 멤버와 의미를 그대로 유지하고, C1 Busy/Stale를 각각 `Busy/C1Returned`, `ConfirmationStale/C1Returned`로 나타내며 새 fresh opaque를 같은 실제 result의 사적 등록 구성으로 붙이는 것이다. C2 호출 단계는 원본 `C2Invoked`, 성공은 `Completed` phase로 기록한다. 결과가 만들어지지 않는 예외/guard 이전 거절은 result enum을 새로 추가하지 말고 원본 계약의 예외/사전 거절 규칙에 둔다. 이 차이를 수정한 새 exact map 지문을 대상으로 재검토해야 하며, 이번 Draft로 결과 호출부를 새 enum에 맞춰 구현하면 안 된다.

## 그 밖에 확인한 경계

정확 개정의 runtime 8개 경로 및 신규 bridge/test/fixture/meta 후보는 원래 허용 범위와 일치하며 Q-A/Q-B의 실제 수정은 별도 amendment 조건으로 유지한다. C1/Profile algorithm·result/proof·issuer, asmdef/friend/public ABI, 제품 asset/settings 및 기존 meta는 금지로 남는다. 새 C4 source/meta가 현재 R11 14-file set과 기존 C3의 884입력 원장에 포함되지 않으므로, 수정 후에는 별도 source manifest·C4 exact selection·input capture 및 QA protocol이 필요하다.

Fresh API는 lower가 Hub 타입을 참조하지 않고, private CWT/history를 통한 원본 actual request·C3 Completed·C4 소비·Owner 객체·pair/root/generation·thread 연결을 유지한다. Q-A/Q-B에는 typed C1 identity/proof/root가 전달되지 않고 Hub-only capability/reservation이 전달된다. owner의 실제 Q-B reserve→Q-A prepare/ack→Q-B commit→같은 capability consume 순서가 lower Complete 호출을 소스상 지배한다는 증거로 결합할 수 있다. lower가 Hub acknowledgment를 역조회하지 않는 것은 이 구조에서 의도된 의존 방향이다. 단, 구현 후 정확 caller/reference escape 감사를 통해 lower reservation이 다른 호출 경로에서 조기 Complete되지 않음을 입증해야 한다. owner의 새 slot은 lower Complete 성공 전 외부 live 권한이 아니어야 한다.

Opaque 결과·handback/reservation의 CWT 등록, actual C1/C2 typed 원본 보존, null 조합을 닫은 matrix, default enum 값 0 거절, same-result one-consumer와 partial failure 시 fault 기록 후 양측 종료는 설계 후보로 구체화되어 있다. C1 실제 closed outcome/state 다섯 조합과 C2 Busy의 post-DiskPrepared terminal 분류가 서로 섞이지 않는다. Gate finally, native callback 앞 fault 원장 기록, clean late loser의 비변경 거절, CWT append history 보존도 제안에 적혀 있다. 그러나 이들은 source/fixture가 없는 planned 계약이며 정적 또는 실제 실행 통과로 검증되지 않았다.

Thread projection·issuer anchor·RequestIssued 참조는 현재 C3 소스에 없는 계획 상태다. 한정 수용은 unsupported/untrusted thread의 무소비 문맥 거절과 trusted original thread의 full validation/손상 containment를 선택하지만, 이를 현재 구현으로 표현할 수 없다. result map P1을 제외하면 설계 범위 자체에 추가 P0/P1은 찾지 못했다. C3 610 실행/선행 gate 및 C4 구현 독립 검증은 별도 미완료이며, 이번 검토는 C4 Approved·C3 수용·실행 결과를 주장하지 않는다.

소스·QA·기존 문서/원장·Git·네트워크·Unity·컴파일을 변경하거나 실행하지 않았다.
