# C4 Play 현재 소스 지문 결속 한정 계약

- 상태: **Approved — 지정된 시험 결속 구현만 승인; Unity 재검증과 C4 통합 수용은 미완료**. 2026-09-30, 아스트라, 실제 `gpt-6-astra`.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-009/010`; 기존 Play 행동의 `AC-M5D7QC4-001..008`은 그대로 유지한다.
- 설계: `docs/proposals/2026-09-30-c4-play-r2-current-source-pin-design.md`, SHA-256 `437E933ADA4D49D069ACB2CE63807B5A10E76A9B3366FEB28EC32B78C7229B39`.
- 독립 설계 검수: `docs/verification/2026-09-30-c4-play-r2-current-source-pin-design-luna-review.md`, SHA-256 `F57543209039358CBE7145AE8E7CD3435A0A09BCFC34951EB3F1D64A5E3D25D4`, P0/P1=0/0.
- 선행: Approved C4 r4와 C4 Play R1 한정 계약. 두 계약의 행동·증거 요구는 이 문서로 약화되지 않는다.

별도 진단 `artifacts/c4-play-r2-diagnostic.xml`은 32건 모두 `C4ActualExecutionPlayFixtureV1.cs:991`의 선행 지문 검사에서 실패했다. Bridge의 역사 v1 예상 지문은 `04709FA5E07A1417FF1BFF5D7829441C7C990F35B3896184C2BCC68A2CBF46BB`이고, 현행 v11 원장과 실제 파일 지문은 `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`다. Owner의 v1·v11·실제 지문은 모두 `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`다. 입력 1017개는 이 진단 전후 일치했다. 이 선행 실패는 R1 제품·시험 보정의 동작 결과를 판정하지 않는다. 기존 19/32 실행과 새 0/32 진단의 원본 출력은 보존한다.

## 허용 구현

1. 변경 가능한 시험 소스는 `Assets/AcadeGameMaker/Tests/PlayMode/InputUnity/C4ActualExecutionPlayFixtureV1.cs` 한 파일이다. `AssertFrozenNoOpSingleCallSource`의 두 Runtime 소스 결속을 최종 현재 원장 `artifacts/c4-frozen-source-manifest-v12.json`으로 옮긴다. Bridge 예상 지문만 현행 실제 지문으로 맞추고 Owner 예상 지문은 그대로 둔다. 각 정확한 경로가 원장에 유일하며 `Kind=Runtime`, 원장 SHA, 실제 파일 바이트 SHA가 모두 일치해야 한다.
2. 같은 fixture의 `FrozenNoSameCaseSavePointers`가 검증하는 부모·fixture 현재 지문도 v11에서 v12로 옮긴다. fixture 최종 바이트 지문을 발급한 뒤 v12를 비재귀적으로 동결한다. 일반 저장소의 별도 역사 검증 `AssertFrozenOrdinaryWriterSource`는 v1 원장 결속을 유지한다.
3. no-op 실행 표현식의 정확 포함 검사와 Bridge 네 개·Owner 세 개 호출 토큰의 각각 정확히 한 번 출현 검사를 그대로 유지한다. 검사를 삭제하거나 느슨하게 하거나 건너뛰지 않는다. 제품 Runtime, Play 부모·32개 이름·선택·기대 행동·값, QA 판정 규칙은 변경하지 않는다. 실제 소스가 단일 호출 조건과 다르면 별도 실패로 기록하고 재설계한다.

## 증거와 판정

Edit fixture의 현재 원장 자기 결속만 v11에서 v12로 갱신할 수 있다. 기대 행은 188개 행 내용·순서를 유지한 새 v7로 발급하며 두 fixture SourceFiles 지문만 새 실제 바이트에 맞춘다. Play 선택 v3의 32개 이름과 부모 지문은 변경하지 않는다. 소스 원장 v12와 입력 경로 v12는 기존 v11 입력 경로·사유를 보존하면서 새 증거만 추가하고 `CreateNew`로 발급한다. 기존 원장·입력·선택·XML·로그·큐 결과를 덮어쓰지 않는다.

루나는 수정 fixture의 정확 경로·종류·중복 부재·실물 SHA·일곱 호출 토큰, 두 현재 원장 참조와 역사 v1 참조 보존을 독립 정적 검수한다. 이후 새 Play 진단은 별도 출력 경로에서 입력 전후 동일성, 32개 정확 이름, 실제 원시 종료와 전체 실패 행을 기록한다. 진단 통과만으로 최종 수용하지 않는다. 진단 결과에 새 실패가 나타나면 원인을 별도로 분류한다. 최종 9회 큐는 새 source/input/행 동결과 기존 필수 선택·실제 통과가 필요하며 `AC-M5D7QC4-009/010`에 결속한다.
