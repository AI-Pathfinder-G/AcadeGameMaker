# C3 lower confirmed-request 런타임 독립 정적 검수

- 검수자: Luna
- 날짜: 2026-09-29
- runtime SHA-256: `8B7FC5DB1BCC11E533A7A8A9A85C2AA43B5392BD1E734C95EBAF9A79A74456BF`
- 시험 SHA-256: `23E3A2B10FE6CFD36C835E7E8E9DD76CDEC1066A5B59F781DAA1D72D6E29C68A`
- 설계 기준 SHA-256: `EAA6CBE69CACC2BCA91515296768FC6E0C76F3C2BDCF604AF661D77A70A3218D`
- 범위: 정적 소스·시험 검수. Unity 실행·소스 수정·상위 수용은 수행하지 않음.

## 판정

P0는 없다. `IssueNoConfirmationRequired`와 재검증 경로는 발급 registry, exact
adapter/router/owner/epoch, capture 검증, 현재 authenticated root 재조회 및
classification을 확인한다. `ConditionalWeakTable` witness와 resolution pair
검증, 세 leaf 및 projection 일치 검증은 설계의 방향과 일치한다. lower가 Q-B
발급이나 C4 실행 결과를 증명한다고 주장해서도 안 된다는 주석과 API 범위도
대체로 유지된다.

## P1 — 예약 전 검증과 예외의 terminality

`ReserveExecutionCommit`은 exact request/cohort/generation을 확인한 뒤
`ConfirmedRequestWitness.State`를 먼저 CAS한다. 그러나 이 경계에서는 보관된
capture의 `Validate`, projection/leaf agreement, 현재 authenticated root를
검사하지 않는다. `CompleteExecutionCommit`에서 이를 검사하지만, 그 검사가
실패해도 state는 `CommitReserved`로 남아 원래 reservation으로 재시도할 수 있다.
설계 기준은 예약·완료 중 불일치나 예외를 lower `Closed`로 보내고 `Issued`로
복구하지 않는다고 명시한다. 따라서 최소 보정은 예약 CAS 전에 capture와 현재
root를 검증하고, reservation 이후 검증/등록 예외는 반드시 terminal close하는
것이다. 이 보정 전에는 `AC-M5D7QC3-006`, `AC-M5D7QC3-008` 수용 근거로 삼을 수
없다.

## P1 — C4 이전 identity 추출 경계

`ProfileNewGameCaptureResultV1.IdentityFor(owner, epoch)`가 internal identity
반환 API로 존재한다. owner/epoch를 요구하더라도 이는 lower가 capture identity를
직접 꺼내게 하며, 승인 설계가 C4 승인 뒤에만 허용한 execution identity 추출
경계를 앞당긴다. 상위 Q-B 권한이나 C4 report를 이 메서드로 만들 수 있다는
주장은 금지되어야 하며, Astra가 이 API를 제거하거나 C4 승인 전 접근 불가한
구조로 명시하기 전에는 `AC-M5D7QC3-009`와 C4 선행조건을 충족한 것으로 볼 수
없다.

## 시험 증거의 남은 공백

현재 시험은 forged request, forged reservation, foreign owner/epoch 및 capture
owner reflection을 확인한다. 다음 음성 행은 아직 직접 확인되지 않았다.

- witness의 root/capture/outcome/classification 필드 reflection 변조와 capture
  내부 projection/leaf 변조 후 발급·예약·완료 거부
- forged capture/result clone 및 impossible resolution disposition/request 조합
- capture 후 authenticated root/cohort 변경, 예약 후 root/projection 불일치에서
  즉시 `Closed`가 되고 원래 reservation으로 재완료할 수 없는지
- current full three-leaf projection agreement를 깨는 실제 fixture와 CWT pair
  불일치의 조합

이는 정적 guard의 존재를 부정하지 않지만, 해당 음성 증거가 없으므로
`AC-M5D7QC3-001`, `AC-M5D7QC3-004`, `AC-M5D7QC3-008`의 실행 수용 범위를
주장할 수 없다. 실제 Unity 실행 전에는 이 공백을 보완해야 한다.

## 상태

- P0: 0
- P1: 2 — 예약 전 현재 증거 검증·예외 terminal close, C4 이전 `IdentityFor` 경계
- lower 단위는 C3 전체, Q-B 인증, owner/rearm 또는 C4를 수용하지 않는다.
