# C4 Edit R7 현재 소스 지문 구현 정적 검토

- 대상: `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/C4ActualExecutionEditFixtureV1.cs`
- 현재 SHA-256: `DEDF385EC466A7B7A7CED2F75AE3EE70E290A4AFDC1C40A4AB7C0F34F15A0668`
- 이전 SHA-256: `16DEB90F87C0E66A6DFD8C04E9C0DB6EC91F59F0051690853B61FA5C49489B0C`
- 승인 기준: `docs/specs/work-contracts/2026-09-30-c4-edit-r7-current-source-pin-recovery-amendment.md` (승인 SHA는 별도 승인 기록 기준)
- 범위: fixture 한 파일의 정적 검토만 수행했다. 원장 v16은 아직 발급 전이며 Unity 실행이나 시험 수용은 하지 않았다.

## 판정

P0 0건, P1 0건이다. 현 파일에서 이전 지문을 재구성하려고 Busy·NoOp의 옛 Bridge 지문과 각 참조 위치의 원장 v1/v12 경로로 되돌렸을 때 이전 SHA가 정확히 재현됐다. 네 원장 경로 참조의 위치별 치환과 두 Bridge SHA 치환만으로 바이트가 복원되므로, 의도한 제한 변경 외의 fixture 수정은 확인되지 않았다.

Busy 검사는 v16을 읽고 ResetService `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`, DiskTransaction `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, Bridge `52E00FE10FDEAFB8C192CF7F92B2EA428C16A202D640BEAF79C8F58E6704F32A`를 각각 `Runtime` 경로·원장값·실제 파일 바이트와 대조한다. 파일 잠금, Busy 예외, disk 시작 뒤 fresh 분기와 handback checkpoint 순서 검사는 유지된다.

NoOp 검사는 v16을 읽고 Bridge의 현재 SHA와 Owner `2838102F468C325E9A41C5302E753BB358179F63F0A84BEE40AFC282292C9BA7`을 대조한다. 실제 파일 바이트 SHA 비교, NoOp 표현식, Bridge 호출 토큰 네 개와 Owner 호출 토큰 세 개의 정확히 한 번 검사가 남아 있다. 일반 저장소 검사는 유일하게 v1을 계속 읽으며 두 Runtime의 과거 지문을 검사한다. 두 Edit fixture 자기 참조는 v16으로 바뀌었다. 결과적으로 fixture 안의 원장 참조는 v16 네 곳과 보존된 ordinary-writer v1 한 곳이다.

## 한계 및 실행 전 조건

v16 원장이 발급되기 전이라 새 원장에 기록될 fixture SHA, `Kind`, `AllowedChanges`, 입력 목록 및 행 v8과의 결속은 아직 검토하지 않았다. 실행 전 동결 검토에서 이 결속과 함께 부모 시험·선택 집합·188개 기대 행이 유지되는지 확인해야 한다. 이번 결과는 이전 XML의 134 실패를 정정하거나 제품 동작을 입증하지 않는다.
