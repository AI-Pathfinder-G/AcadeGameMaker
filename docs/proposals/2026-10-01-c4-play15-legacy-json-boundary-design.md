# C4 Play15 선행 진단 출력의 행 판별 경계 제안

상태: 제안. 이 문서는 승인이나 구현 권한을 부여하지 않는다. 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`; 선행 시험의 `AC-M5D7QC3-005` 보존.

## 확인한 원인과 범위

- v14 큐 결과 `artifacts/c4-final-validation-queue-result-v12.json`은 여섯째 `c4-play15`에서 멈췄다. `artifacts/c4-play15.xml`의 15개 NUnit 사례는 모두 통과하고 native/QA 종료는 0이다. `artifacts/c4-play15-required-row-comparison.json`에는 계획·관측 C4 행 0개, `SourceMatched=true`, `InvalidIds` 두 개, `EvidenceMatched=false`가 남았다. 이 실패를 C4 제품 동작의 반례로 분류하지 않는다.
- 선행 고정 시험 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs:38-56`은 두 사례에서 `TestContext.Out`에 `C5-same-cancel-absent`와 `C5-same-cancel-present` 진단 JSON을 쓴다. 둘 다 `schemaVersion`과 C4 행 필드가 없다. `artifacts/c4-verify-required-rows.ps1:345-347`은 `{"id":`로 시작하는 모든 줄을 먼저 C4 후보로 읽고, 기존 `authorityChecked` 예외에 맞지 않는 두 줄을 형식 오류로 집계한다. Play15 동결 선택에는 두 사례가 모두 있으며 Play610 동결 선택에는 이 시험 클래스가 없다. 같은 소스의 `C3-rearm-render-stale-reentry` 출력까지 일반 허용할 근거는 없다.
- `docs/specs/work-contracts/2026-09-29-c4-qa-evidence-protocol.md:54,68`의 C4 행 정확 형식과 `docs/specs/work-contracts/2026-09-30-c4-remaining149-zero-row-verifier-recovery-amendment.md:12,20-21`의 0행 예상 밖·형식 오류 거절을 유지한다.

## 승인 전 필요한 한정 계약

1. `artifacts/c4-verify-required-rows.ps1`의 XML 출력 분류 한 곳에만 선행 진단의 정확 분기를 둔다. 적용 조건은 `RequiredRowIds=[]`, 동결된 Play15 선택 경로와 SHA-256 `artifacts/c3-r11-playmode-focused-selection.json` / `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01`, 해당 선택의 정확한 두 완전 수식 사례명, 원본 시험 소스 SHA-256 `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`의 현재 원장 결속을 모두 만족할 때뿐이다. XML에는 두 완전 수식 사례명별 `test-case`가 정확히 하나씩 있어야 하고 두 부모 모두 `result=Passed`여야 한다. 진단 출력은 메서드 마지막 Rearm 단언보다 먼저 기록되므로 출력 존재만으로 통과를 인정하지 않는다. 줄은 원본에서 실제 관측한 `id/status/observedFields/currentMemoryAbsent/wholeGameplaySessionClaimed` 다섯 키와 값의 정확한 두 형태만 허용하고 중복 JSON 키를 거절한다. `absent`는 `(False)`, `present`는 `(True)` 사례에만 대응하며 각 하나씩만 인정한다. 원문 줄, 사례, 키, 값, 횟수 또는 부모 결과 중 하나라도 달라지면 기존 형식 오류나 별도 불일치로 실패한다.
2. 이 분기는 선행 진단을 C4 `Rows`나 계획·도달·통과 개수에 넣지 않는다. 분기에 들지 않은 모든 `{"id":` 줄은 기존 `C4-Parse`/`C4-Record`를 거친다. 따라서 0행 실행의 정확 형식 C4 추가 행은 `UnexpectedIds`, 형식 불량 C4 행은 `InvalidIds`로 계속 실패한다. `schemaVersion` 없는 JSON 전반, `C3-...` 출력, ID 접두사만으로 통과시키는 완화는 금지한다. 기존 `authorityChecked` 역사 분기의 범위도 이번 변경에서 넓히지 않는다.
3. 먼저 도구만 대상으로 회귀한다. 실제 Play15 XML의 두 정확 진단 줄과 유일한 통과 부모 둘은 0행 수용, 두 줄 각각의 변조·교환·중복·중복 JSON 키·다른 사례 귀속, 부모 한쪽의 `Failed` 결과 또는 동일 완전 수식 부모 `test-case` 중복은 실패해야 한다. 0행 예상 밖 정상 C4 행과 형식 불량 C4 행은 보고서를 남긴 채 종료 1이어야 한다. 기존 Remaining149 0행 양성, 비영 Play 35/35와 누락·불일치 음성, Matrix91의 377/377을 다시 확인한다. Play610에서는 이 두 C5 사례가 선택되지 않았음을 선택 원장과 XML로 따로 확인하며 새로운 선행 진단을 추측으로 면제하지 않는다.
4. 승인 뒤 구현 허용 후보는 위 검증기 한 파일과 `artifacts/c4-final-validation-queue.ps1:11`의 결과 경로 상수 한 줄뿐이다. 현재 그 줄은 이미 존재하는 `artifacts/c4-final-validation-queue-result-v12.json`만 허용하므로, 후속 큐가 새 `artifacts/c4-final-validation-queue-result-v13.json`을 한 번 생성할 수 있도록 그 경로만 바꾼다. 큐의 나머지 검증·중단 규칙은 유지한다. 원본 v14 계획, 결과 v12, XML, 입력 v18, 선택, 시험 소스, C4 188행 원장과 제품 코드는 불변이다. 도구 회귀와 독립 검수 후 변경된 **두 도구 각각의 새 SHA**를 새 source manifest의 파일 결속과 input 직접 입력에 모두 묶고, 이미 존재하는 첫 여섯 실행 출력을 재사용하지 않는 후속 계획을 발급한다. 실행 전 아홉 선택의 현재 지문·새 출력 부재를 다시 확인한다. 실제 전체 수용은 아홉 실행의 native/QA/바깥 종료 0, 이름·행·입력 전후 지문, 독립 검수까지 요구한다.

현재 단계에서는 도구 수정, Unity 재실행, 제품 수용을 수행하지 않았다.
