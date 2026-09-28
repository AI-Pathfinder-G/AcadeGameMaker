# VD-09 M5D7Q C3L focused R3 Luna 독립 검토

- 검수자: Luna
- 시험: `c3l-r3-focused.xml`, 실제 Editor 종료 코드 0
- 시험 source SHA: `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameObservationLeaseV1Tests.cs` `E899F8531763CD47BE3366619A31CD91700F65EF6B2770339728B59E66E1E1AA`
- XML SHA-256: `4D11BF36841F6399452D9AF8FC455F3A15301825CC4B2874B8BEB4224BE4220F`

## 판정

R3 focused 실행은 **11/11 통과**, 실패·건너뜀·판정 불가 0, expected name 차이 0, 중복 0이다. before/after 입력은 각각 874개이고 실제 경로·SHA 차이 0이다. R2의 lock-directory fixture setup 실패는 보정 후 해소되었다.

## 통과 범위

- **AC-M5D7QC3-001 계열 / AC-M5D7QC3L-001:** foreign real receipt/actions가 원래 root를 반환하지 못하고, predecessor body reconstruction 지문을 확인했다.
- **AC-M5D7QC3L-002:** 실제 held lock의 5초 timeout Busy와 deadline 전 early release Captured를 확인했다.
- **AC-M5D7QC3L-003:** actual timeout issuer와 원인·wrapper HRESULT 변조/가짜·반사 timeout 거부, root-file, lock-directory, barrier file/directory, held primary unreadable, relative/filesystem-root를 확인했다.
- **AC-M5D7QC3L-004:** 실제 capture 발급, 결과 classification 변조, leaf 순서/hash 및 decoded projection 변조 fail-closed를 확인했다.

정확한 11개 qualified name과 selection은 `artifacts/c3l-r3-verification.json` 및 `artifacts/c3l-focused-selection.json`에 기록되어 있다.

## 보류

이 결과는 focused C3L 실행 증거다. **AC-M5D7QC3L-005와 전체 C3 수용은 보류**한다. 현재 별도 실행 중인 EditMode 562+51과 PlayMode 610 결과가 필요하며, 그 전에는 C3 전체 또는 AC-M5D7QC3-010 통과를 주장하지 않는다. R1 컴파일 실패와 R2 fixture 실패 기록은 역사로 보존한다.
