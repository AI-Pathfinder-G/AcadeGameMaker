# C3L lower capture agreement 사전 검수 — Luna

날짜: 2026-09-28. 역할: GPT Luna 독립 소스 사전 검수.

## 지문

- `Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs`
  SHA-256: `719F81DE9975E6961D563C85A5F324BB7C25AE5BFFB37E499BB60BD14BF3868E`
- `Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs`
  SHA-256: `45B957A4B2739968422BCAA10A23EF94D4103B336FB46A41AB288C3173F5EA44`
- 기존 서비스 본문 보존 재구성 SHA-256: `2E1DCE184495D4AABAC135AFC6910121A94D69498A85669A7D7EA1BD5E554098`
- 기존 디스크 본문 보존 재구성 SHA-256: `168D1539EAF359C3242ECABADAC0776956370E52105007D9224566F7F9C3FDC4`

## 확인된 보강

`ProfileResetConfirmationCaptureV1.Validate`는 발급 witness와 identity,
세 leaf, projection 유무, decoder `Validate`, classification, revision을
대조한다. `ProfileNewGameCaptureResultV1.Validate`도 owner·epoch·root·capture·
outcome·classification witness와 실제 identity root를 대조하며, lower C3는
인증된 timeout만 `Busy`로 변환한다. 서비스와 디스크의 기존 본문 보존 지문은
제공된 재구성 값과 일치하는 것으로 확인했다.

## 남은 P1

- **AC-M5D7QC3L-003 / AC-M5D7QC3-001 — P1:**
  `ProfileResetConfirmationCaptureV1.Mint(...)`와
  `ProfileNewGameCaptureResultV1.Captured(...)`/`Failure(...)`가 `internal`
  정적 발급 함수다. 같은 내부 경계의 호출자가 실제 `CaptureConfirmationObservation`
  lease나 실제 C3 owner/epoch 발급 경계를 거치지 않고 임의의 identity, leaves,
  owner, epoch, root, outcome을 넣어 CWT witness가 등록된 결과를 만들 수 있다.
  이후 `Validate`는 그 직접 발급값과만 비교하므로 forged issuer를 구분하지
  못한다. 발급 함수를 실제 observation/C3 경계 내부로 닫거나 private issuer
  witness를 요구해야 한다.

이는 capture agreement 필드 검증의 추가와 별개인 발급 권한 결함이다. 실제
fixture·Unity 시험은 실행하지 않았다.

## 판정

agreement 내부 정합성 검수는 정적 기준 PASS이나, 발급 provenance 결함으로
현재 사전 검수는 **P0=0/P1=1**이다. C3L 전체 수용과
`AC-M5D7QC3L-005` 통과는 주장하지 않는다.
