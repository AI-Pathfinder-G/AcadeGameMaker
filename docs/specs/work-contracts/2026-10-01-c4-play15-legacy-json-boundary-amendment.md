# C4 Play15 선행 진단 JSON 행 판별 한정 계약

- 상태: **Approved — 아래 검증 도구·증거 결속·재실행만 승인; 전체 C4 수용은 새 실제 결과와 독립 검수 전까지 보류**. 2026-10-01, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`, 공동 `AC-M5D7QC3-007/008`; 선행 시험 `AC-M5D7QC3-005`의 기존 증거를 보존한다.
- 설계: `docs/proposals/2026-10-01-c4-play15-legacy-json-boundary-design.md`, SHA-256 `6E33C4E112ADC228D7EF01F7CB4222956CEC5E960D054E88276F422331BBCA84`.
- 독립 설계 검수: `docs/verification/2026-10-01-c4-play15-legacy-json-boundary-luna-r3-review.md`, SHA-256 `FEE027A90A1B3736200062A77424F3114A40DE65B73F4EF4D63B96D6C261D37D`, P0/P1=0/0. 이전 초안의 결과 경로와 부모 시험 결속 P1은 r3에서 닫혔다.
- 원본 실패: `artifacts/c4-final-validation-queue-result-v12.json`, SHA-256 `4BCC11423405BADD3F3853F1AAD6F2970ECDCFB7C20E7E1420EF11CCF688D871`. 첫 다섯 묶음은 통과했고 여섯째 `c4-play15`에서 행 검증 실패로 중단했다. 해당 XML SHA-256 `680F8DEAFC605F10392E1972F1FDAF56242C7F7FDF1746937C8E2E4CC2875843`은 15/15 통과, native/QA/바깥 종료 0이다. 행 비교 SHA-256 `55AB34E4CF7CB33A30EDA883521DF38E2B62CE17647DDE3D1C58D8EC553C4705`는 C4 필수 행 0개, `InvalidIds` 2개, `EvidenceMatched=false`를 기록했다. 원본 출력과 v14 계획·v18 입력은 수정하거나 성공으로 재분류하지 않는다.

## 승인 범위

1. `artifacts/c4-verify-required-rows.ps1`의 XML `{"id":` 출력 판별 한 곳에만 두 C5 선행 진단의 정확 분기를 추가한다. 적용은 계획의 `RequiredRowIds=[]`, 고정 Play15 선택 경로 `artifacts/c3-r11-playmode-focused-selection.json` 및 SHA-256 `C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01`, 현재 시험 소스 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/NewGameConfirmationOwnerPlayModeTests.cs` SHA-256 `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`를 모두 확인한 경우로 제한한다. 완전 수식 부모 `AC005_SameActualCancelPreservesCompleteObservedFixtureSnapshot(False)`와 `(True)`는 XML에 각각 정확히 한 번 있어야 하고 각각 최종 `result=Passed`여야 한다.
2. 두 부모의 출력은 `id/status/observedFields/currentMemoryAbsent/wholeGameplaySessionClaimed` 다섯 키의 정확한 두 원문 형태만 각 한 번 인정한다. `False`는 `C5-same-cancel-absent`, `True`는 `C5-same-cancel-present`이고, 나머지 값은 각각 `passed`, 정수 `183`, 불리언 `true`, 불리언 `false`다. 중복 JSON 키·추가/누락 키·값/형식 차이·다른 부모 귀속·출력 누락/중복·부모 실패/중복은 거부한다. 이 두 진단은 C4 행 `Rows`와 계획·도달·통과 개수에 포함하지 않는다.
3. 위 정확 분기에 들지 않은 모든 `{"id":` 출력은 기존 엄격 파서와 C4 기록 검증을 거친다. 형식이 맞는 예상 밖 C4 행은 `UnexpectedIds`, 형식 불량 행은 `InvalidIds`로 보고하며 종료 1이어야 한다. 기존 `authorityChecked` 역사 분기, 0행 및 비영 행의 다른 규칙, Matrix91의 C3 377행 대조는 변경하지 않는다. 다른 C3 진단이나 `schemaVersion` 누락 JSON을 일반 허용하지 않는다. 제품·시험 fixture·선택·C4 188행 원장도 변경하지 않는다.
4. 먼저 원본 불변 Play15 XML을 대상으로 0행 양성을 확인하고, 두 원문 각각의 변조·교환·누락·중복, JSON 중복 키, 부모 실패·중복·오귀속을 음성 회귀로 확인한다. 정상 형식의 예상 밖 C4 행과 형식 불량 C4 행은 보고서와 종료 1을 요구한다. 기존 Remaining149 0행 양성, 비영 Play 35행 양성/누락·불일치 음성, Matrix91 C3 377행 양성도 유지한다. 모든 회귀 입력·출력·SHA·종료·판정을 새 경로에 기록하고 루나가 독립 검수한다.
5. `artifacts/c4-final-validation-queue.ps1:11`의 결과 허용 경로만 이미 존재하는 `artifacts/c4-final-validation-queue-result-v12.json`에서 새 `artifacts/c4-final-validation-queue-result-v13.json`으로 바꾼다. 변경된 두 도구의 실제 SHA와 본 계약·설계·독립 검수·회귀 근거를 새 v19 소스 원장과 입력 목록에 결속한다. v18의 불변 경로·사유·역사 출력은 보존하고 v19 파일은 `CreateNew`로 발급한다. 새 계획 v15는 아홉 선택의 순서·이름·사례 수 `139/32/5/91/149/15/562/51/610`, 행 배분·잠금·중단·종료 판정을 유지한다. 이미 출력이 있는 첫 여섯 실행은 새 stem과 새 출력 경로를 쓰고, 뒤 세 실행은 모든 출력 부재를 확인한 경우에만 기존 stem을 유지한다. 결과 v13과 모든 새 출력 경로의 부재를 실행 전에 확인한다.

`AC-M5D7QC4-009/010` 및 공동 `AC-M5D7QC3-007/008`의 전체 수용은 새 아홉 묶음의 정확 시험 이름·개수, native/QA/바깥 종료 0, C4 필수 행과 C3 377행, 각 실행 전후 동결 입력 일치, 루나 독립 검수가 모두 충족된 뒤 아스트라가 별도로 판정한다. 새 실패는 원본을 보존하고 새 원인으로 분류한다.
